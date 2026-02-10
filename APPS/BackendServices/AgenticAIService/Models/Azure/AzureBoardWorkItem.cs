namespace AgenticAIService.Models.Azure
{
    // Minimal WorkItem class for internal usage
    public class AzureBoardWorkItem
    {
        // System fields
        public int Id { get; set; }                                // [System.Id]
        public string? Title { get; set; }                         // [System.Title]
        public string? WorkItemType { get; set; }                  // [System.WorkItemType]
        public string? State { get; set; }                         // [System.State]
        public string? Reason { get; set; }                        // [System.Reason]
        public string? AssignedTo { get; set; }                    // [System.AssignedTo] (usually display name or UPN)
        public string? AreaPath { get; set; }                      // [System.AreaPath]
        public string? IterationPath { get; set; }                 // [System.IterationPath]

        public DateTime? CreatedDate { get; set; }                 // [System.CreatedDate]
        public DateTime? ChangedDate { get; set; }                 // [System.ChangedDate]

        // Work tracking fields
        public int? Priority { get; set; }                         // [Microsoft.VSTS.Common.Priority]
        public double? StoryPoints { get; set; }                   // [Microsoft.VSTS.Scheduling.StoryPoints]

        public double? OriginalEstimate { get; set; }              // [Microsoft.VSTS.Scheduling.OriginalEstimate]
        public double? RemainingWork { get; set; }                 // [Microsoft.VSTS.Scheduling.RemainingWork]
        public double? CompletedWork { get; set; }                 // [Microsoft.VSTS.Scheduling.CompletedWork]

        public DateTime? TargetDate { get; set; }                  // [Microsoft.VSTS.Scheduling.TargetDate]
        public DateTime? DueDate { get; set; }                     // [Microsoft.VSTS.Common.DueDate]

        public string? Tags { get; set; }                          // [System.Tags] (semicolon-separated)
    }
}
