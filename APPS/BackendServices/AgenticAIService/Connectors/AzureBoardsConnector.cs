using AgenticAIService.Models.Azure;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
//using Microsoft.TeamFoundation.Releases.WebApi;

using System.Text;

namespace AgenticAIService.Connectors
{
    public class AzureBoardConnector
    {
        private readonly WorkItemTrackingHttpClient _client;
        private readonly GitHttpClient _gitClient;
        private readonly BuildHttpClient _buildClient;
        private readonly WikiHttpClient _wikiClient;
       // private readonly ReleaseHttpClient _releaseClient;

        private readonly string _projectName;

        /// <summary>
        /// Initializes the AzureBoardConnector
        /// </summary>
        /// <param name="orgUrl">Organization URL e.g. https://dev.azure.com/{org}</param>
        /// <param name="personalAccessToken">PAT token</param>
        /// <param name="projectName">Azure DevOps project name</param>
        public AzureBoardConnector(string orgUrl, string personalAccessToken, string projectName)
        {
            _projectName = projectName;

            var credentials = new VssBasicCredential(string.Empty, personalAccessToken);
            var connection = new VssConnection(new Uri(orgUrl), credentials);

            _client = connection.GetClient<WorkItemTrackingHttpClient>();
            _gitClient = connection.GetClient<GitHttpClient>();
            _buildClient = connection.GetClient<BuildHttpClient>();
            _wikiClient = connection.GetClient<WikiHttpClient>();
           // _releaseClient = connection.GetClient<ReleaseHttpClient>();
        }

        #region ================= WORK ITEMS =================

        public async Task<List<AzureBoardWorkItem>> QueryWorkItemsAsync(string wiqlQuery)
        {
            var wiql = new Wiql { Query = wiqlQuery };
            var result = await _client.QueryByWiqlAsync(wiql);

            if (result?.WorkItems == null || result.WorkItems.Count() == 0)
                return new List<AzureBoardWorkItem>();

            var ids = result.WorkItems.Select(w => w.Id).ToList();
            const int pageSize = 100;

            var fetchedWorkItems = new List<WorkItem>();

            for (int i = 0; i < ids.Count; i += pageSize)
            {
                var chunk = ids.Skip(i).Take(pageSize).ToArray();
                if (chunk.Length == 0) break;

                var items = await _client.GetWorkItemsAsync(chunk, expand: WorkItemExpand.Fields);
                if (items != null && items.Count > 0)
                {
                    fetchedWorkItems.AddRange(items);
                }
            }

            if (fetchedWorkItems.Count == 0)
                return new List<AzureBoardWorkItem>();

            var fetchedById = fetchedWorkItems
                .Where(fi => fi?.Id != null)
                .ToDictionary(fi => fi.Id!.Value, fi => fi);

            var orderedItems = ids
                .Where(id => fetchedById.ContainsKey(id))
                .Select(id => fetchedById[id])
                .ToList();

            return orderedItems.Select(i => new AzureBoardWorkItem
            {
                Id = i.Id ?? 0,
                Title = i.Fields.ContainsKey("System.Title") ? i.Fields["System.Title"].ToString() ?? string.Empty : string.Empty,
                State = i.Fields.ContainsKey("System.State") ? i.Fields["System.State"].ToString() ?? string.Empty : string.Empty,
                WorkItemType = i.Fields.ContainsKey("System.WorkItemType") ? i.Fields["System.WorkItemType"].ToString() : string.Empty,
                AssignedTo = i.Fields.ContainsKey("System.AssignedTo") ? i.Fields["System.AssignedTo"].ToString() : null,
                CreatedDate = i.Fields.ContainsKey("System.CreatedDate") && i.Fields["System.CreatedDate"] != null
                  ? Convert.ToDateTime(i.Fields["System.CreatedDate"])
                  : null,
                ChangedDate = i.Fields.ContainsKey("System.ChangedDate") && i.Fields["System.ChangedDate"] != null
                  ? Convert.ToDateTime(i.Fields["System.ChangedDate"])
                  : null,
                CompletedWork = i.Fields.ContainsKey("Microsoft.VSTS.Scheduling.CompletedWork") && i.Fields["Microsoft.VSTS.Scheduling.CompletedWork"] != null
                  ? Convert.ToDouble(i.Fields["Microsoft.VSTS.Scheduling.CompletedWork"])
                  : 0,
                RemainingWork = i.Fields.ContainsKey("Microsoft.VSTS.Scheduling.RemainingWork") && i.Fields["Microsoft.VSTS.Scheduling.RemainingWork"] != null
                  ? Convert.ToDouble(i.Fields["Microsoft.VSTS.Scheduling.RemainingWork"])
                  : 0,
                Priority = i.Fields.ContainsKey("Microsoft.VSTS.Common.Priority") && i.Fields["Microsoft.VSTS.Common.Priority"] != null
                  ? Convert.ToInt32(i.Fields["Microsoft.VSTS.Common.Priority"])
                  : null,
                StoryPoints = i.Fields.ContainsKey("Microsoft.VSTS.Scheduling.StoryPoints") && i.Fields["Microsoft.VSTS.Scheduling.StoryPoints"] != null
                  ? Convert.ToDouble(i.Fields["Microsoft.VSTS.Scheduling.StoryPoints"])
                  : null,
                OriginalEstimate = i.Fields.ContainsKey("Microsoft.VSTS.Scheduling.OriginalEstimate") && i.Fields["Microsoft.VSTS.Scheduling.OriginalEstimate"] != null
                  ? Convert.ToDouble(i.Fields["Microsoft.VSTS.Scheduling.OriginalEstimate"])
                  : null
            }).ToList();
        }

        //public async Task<List<AzureBoardWorkItem>> GetAllWorkItemsAsync()
        //{
        //    string wiqlQuery = $@"
        //        SELECT [System.Id], [System.Title], [System.State], 
        //               [System.AssignedTo], [Microsoft.VSTS.Scheduling.CompletedWork]
        //        FROM WorkItems
        //        WHERE [System.TeamProject] = '{_projectName}'
        //        AND ([System.CreatedDate] >= @Today - 365 OR [System.ChangedDate] >= @Today - 365) 
        //        AND [System.State] <> 'Closed'
        //        ORDER BY [System.Id] ASC";

        //    return await QueryWorkItemsAsync(wiqlQuery);
        //}

        public async Task<List<AzureBoardWorkItem>> GetAllWorkItemsAsync()
        {
            string wiqlQuery = $@"
        SELECT 
            [System.Id],
            [System.Title],
            [System.WorkItemType],
            [System.State],
            [System.AssignedTo],
            [System.CreatedDate],
            [Microsoft.VSTS.Scheduling.StoryPoints],
            [Microsoft.VSTS.Scheduling.OriginalEstimate],
            [Microsoft.VSTS.Scheduling.CompletedWork]
        FROM WorkItems
        WHERE 
            [System.TeamProject] = '{_projectName}'
            AND ([System.CreatedDate] >= @Today - 365 
                 OR [System.ChangedDate] >= @Today - 365)
            AND [System.State] <> 'Closed'
        ORDER BY [System.Id] ASC";

            return await QueryWorkItemsAsync(wiqlQuery);
        }


        public async Task<AzureBoardWorkItem?> GetWorkItemByIdAsync(int id)
        {
            var item = await _client.GetWorkItemAsync(id, expand: WorkItemExpand.Fields);
            if (item == null) return null;

            return new AzureBoardWorkItem
            {
                Id = item.Id ?? 0,
                Title = item.Fields.ContainsKey("System.Title") ? item.Fields["System.Title"].ToString() : string.Empty,
                State = item.Fields.ContainsKey("System.State") ? item.Fields["System.State"].ToString() : string.Empty,
                WorkItemType = item.Fields.ContainsKey("System.WorkItemType") ? item.Fields["System.WorkItemType"].ToString() : string.Empty,
                AssignedTo = item.Fields.ContainsKey("System.AssignedTo") ? item.Fields["System.AssignedTo"].ToString() : null,
                CreatedDate = item.Fields.ContainsKey("System.CreatedDate") && item.Fields["System.CreatedDate"] != null
                  ? Convert.ToDateTime(item.Fields["System.CreatedDate"])
                  : null,
                ChangedDate = item.Fields.ContainsKey("System.ChangedDate") && item.Fields["System.ChangedDate"] != null
                  ? Convert.ToDateTime(item.Fields["System.ChangedDate"])
                  : null,
                CompletedWork = item.Fields.ContainsKey("Microsoft.VSTS.Scheduling.CompletedWork") && item.Fields["Microsoft.VSTS.Scheduling.CompletedWork"] != null
                  ? Convert.ToDouble(item.Fields["Microsoft.VSTS.Scheduling.CompletedWork"])
                  : 0,
                RemainingWork = item.Fields.ContainsKey("Microsoft.VSTS.Scheduling.RemainingWork") && item.Fields["Microsoft.VSTS.Scheduling.RemainingWork"] != null
                  ? Convert.ToDouble(item.Fields["Microsoft.VSTS.Scheduling.RemainingWork"])
                  : 0,
                Priority = item.Fields.ContainsKey("Microsoft.VSTS.Common.Priority") && item.Fields["Microsoft.VSTS.Common.Priority"] != null
                  ? Convert.ToInt32(item.Fields["Microsoft.VSTS.Common.Priority"])
                  : null,
                StoryPoints = item.Fields.ContainsKey("Microsoft.VSTS.Scheduling.StoryPoints") && item.Fields["Microsoft.VSTS.Scheduling.StoryPoints"] != null
                  ? Convert.ToDouble(item.Fields["Microsoft.VSTS.Scheduling.StoryPoints"])
                  : null,
                OriginalEstimate = item.Fields.ContainsKey("Microsoft.VSTS.Scheduling.OriginalEstimate") && item.Fields["Microsoft.VSTS.Scheduling.OriginalEstimate"] != null
                  ? Convert.ToDouble(item.Fields["Microsoft.VSTS.Scheduling.OriginalEstimate"])
                  : null
            };
        
        }

        #endregion


        #region ================= REPOS =================

        public async Task<List<GitRepository>> GetRepositoriesAsync()
        {
            return await _gitClient.GetRepositoriesAsync(_projectName);
        }

        public async Task<List<GitBranchStats>> GetBranchesAsync(Guid repoId)
        {
            // Fix: Convert Guid to string for GetBranchesAsync
            return await _gitClient.GetBranchesAsync(repoId.ToString());
        }

        public async Task<List<GitCommitRef>> GetRecentCommitsAsync(Guid repoId, int top = 50)
        {
            // Fix: Convert Guid to string for GetCommitsAsync
            return await _gitClient.GetCommitsAsync(
                repoId.ToString(),
                new GitQueryCommitsCriteria(),
                top: top
            );
        }

        //public async Task<string> GetFileContentAsync(Guid repoId, string path, string branchName = "main")
        //{
        //    // Fix: Convert Guid to string for GetItemContentAsync
        //    var stream = await _gitClient.GetItemContentAsync(
        //        repoId.ToString(),
        //        path,
        //        new GitVersionDescriptor
        //        {
        //            VersionType = GitVersionType.Branch,
        //            Version = branchName
        //        });

        //    using var reader = new StreamReader(stream, Encoding.UTF8);
        //    return await reader.ReadToEndAsync();
        //}

        #endregion


        #region ================= PIPELINES (Build) =================

        public async Task<List<BuildDefinitionReference>> GetBuildDefinitionsAsync()
        {
            return await _buildClient.GetDefinitionsAsync(_projectName);
        }

        public async Task<List<Build>> GetRecentBuildsAsync(int top = 50)
        {
            return await _buildClient.GetBuildsAsync(_projectName, top: top);
        }

        public async Task<Build?> GetBuildByIdAsync(int buildId)
        {
            return await _buildClient.GetBuildAsync(_projectName, buildId);
        }

        #endregion


        #region ================= RELEASE PIPELINES =================

        //public async Task<List<ReleaseDefinition>> GetReleaseDefinitionsAsync()
        //{
        //    return await _releaseClient.GetReleaseDefinitionsAsync(_projectName);
        //}

        //public async Task<List<Release>> GetReleasesAsync(int top = 50)
        //{
        //    return await _releaseClient.GetReleasesAsync(_projectName, top: top);
        //}

        #endregion


        #region ================= WIKI =================

        public async Task<List<WikiV2>> GetWikisAsync()
        {
            return await _wikiClient.GetAllWikisAsync(_projectName);
        }

        public async Task<WikiPageResponse> GetWikiPagesAsync(string wikiId)
        {
            return await _wikiClient.GetPageAsync(
                project: _projectName,
                wikiIdentifier: wikiId,
                path: "/",
                recursionLevel: VersionControlRecursionType.Full
            );
        }

        public async Task<string?> GetWikiPageContentAsync(string wikiId, string path)
        {
            var page = await _wikiClient.GetPageTextAsync(
                _projectName,
                wikiIdentifier: wikiId,
                path: path
            );

            using var reader = new StreamReader(page);
            return await reader.ReadToEndAsync();
        }

        #endregion
    }
}
