import React from 'react';
import '../Profile.css';

interface TeamMember {
  id: number;
  name: string;
  email: string;
  designation: string;
  role: string;
  organization: string;
  imageUrl: string;
  phone?: string;
  location?: string;
}

const ProfileCard: React.FC = () => {
  const teamName = 'Team - IntelliTrace';
  
  const teamMembers: TeamMember[] = [
    {
      id: 1,
      name: 'Prashant Akhouri',
      email: 'Prashant.akhouri@winwire.com',
      designation: 'Technical Architect',
      role: 'Technical Architect',
      organization: 'WinWire Technologies',
      imageUrl: '/images/profile/prashant-akhouri.jpg',
      phone: '+91 7330990481',
      location: 'Bengaluru'
    },
    {
      id: 2,
      name: 'Pavani Sowdepalli',
      email: 'Pavani.Sowdepalli@winwire.com',
      designation: 'QA Engineer',
      role: 'QA Engineer',
      organization: 'WinWire Technologies',
      imageUrl: '/images/profile/pavani-sowdepalli.jpg',
      phone: '+91 6303165816',
      location: 'Bengaluru'
    },
    {
      id: 3,
      name: 'Vinod Medepalli',
      email: 'Vinod.Medepalli@winwire.com',
      designation: 'Senior software design engineer',
      role: 'senior software design engineer',
      organization: 'WinWire Technologies',
      imageUrl: '/images/profile/vinod-medepalli.jpg',
      phone: '+91 7671836204',
      location: 'Bengaluru'
    },
    {
      id: 4,
      name: 'Narendar Thajsingh',
      email: 'narendar.thajsingh@winwire.com',
      designation: 'Technical Architect',
      role: 'Tech Arch',
      organization: 'WinWire Technologies',
      imageUrl: '/images/profile/narendar-thajsingh.jpg',
      phone: '+91 9952474547',
      location: 'Hyderabad'
    },
    {
      id: 5,
      name: 'Kishore Jakkani',
      email: 'kishore.jakkani@winwire.com',
      designation: 'Lead QA',
      role: 'Lead QA',
      organization: 'WinWire Technologies',
      imageUrl: '/images/profile/kishore-jakkani.jpg',
      phone: '+91 9133888812',
      location: 'Hyderabad'
    },
    {
      id: 6,
      name: 'Aashutosh Kumar',
      email: 'aashutosh.kumar@winwire.com',
      designation: 'QA Engineer',
      role: 'QA Engineer',
      organization: 'WinWire Technologies',
      imageUrl: '/images/profile/aashutosh-kumar.jpg',
      phone: '+91 6366975300',
      location: 'Bengaluru'
    },
{
        id: 7, 
        name: 'Rehaman Orwai',
        email: 'rehaman.orwai@winwire.com', 
        designation: 'QA Engineer',
        role: 'QA Engineer',
        organization: 'WinWire Technologies',
        imageUrl: '/images/profile/rehaman-orwai.jpg',
        phone: '+91 9380689892',
        location: 'Bengaluru'
      },
      {
        id: 8,
        name: 'Sunil Ksheersagar',
        email: 'sunil.ksheersagar@winwire.com',
        designation: 'Senior Technical Lead',
        role: 'Senior Technical Lead',
        organization: 'WinWire Technologies',
        imageUrl: '/images/profile/sunil-ksheersagar.jpg',
        phone: '+91 9496196109',
        location: 'Hyderabad'
      }

    ];


  return (
    <div className="team-container">
      <h1 className="team-name">{teamName}</h1>
      <div className="team-profiles">
        {teamMembers.map((member) => (
          <div key={member.id} className="team-member-card">
            <div className="member-image">
              <img src={member.imageUrl} alt={member.name} />
            </div>
            <div className="card-content">
              <div className="member-header">
                <h3 className="member-name">{member.name}</h3>
              </div>
              <div className="member-details">
                <div className="detail-item">
                  <label>Designation:</label>
                  <p>{member.designation}</p>
                </div>
                <div className="detail-item">
                  <label>Email:</label>
                  <p>
                    <a href={`mailto:${member.email}`}>{member.email}</a>
                  </p>
                </div>
                {member.phone && (
                  <div className="detail-item">
                    <label>Phone:</label>
                    <p>
                      <a href={`tel:${member.phone}`}>{member.phone}</a>
                    </p>
                  </div>
                )}
                {member.location && (
                  <div className="detail-item">
                    <label>Location:</label>
                    <p>{member.location}</p>
                  </div>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default ProfileCard;