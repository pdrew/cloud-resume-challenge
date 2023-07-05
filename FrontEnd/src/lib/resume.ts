export function getResume() : Resume {
    const positions: Position[] = [
        {
            title: 'Infrastructure Engineer',
            start: 'Apr 2022',
            end: 'Present',
            company: 'Tessitura Network',
            achievements: [
                'Designed CloudFormation template and PowerShell DSC resources for creating a multi-subnet VPC and EC2 instances used to complete Secure Software Report of Validation.',
                'Created monitoring solution for build server instances using Lambda, CloudWatch Agent, Logs, Alarms, Synthetic Canaries and AWS CDK.',
                'Implemented custom patch management for EC2 instances using Systems Manager Automation and Run Commands, deployed and maintained with AWS CDK.'
            ],
            technologies: ['AWS', 'AWS CDK', 'C#', 'PowerShell', 'GoCD', 'Bamboo', 'Linux', 'Bash']
        },
        {
            title: 'Senior Software Engineer',
            start: 'Sep 2020',
            end: 'Apr 2022',
            company: 'Tessitura Network',
            achievements: [
                'Full stack development on an enterprise CRM, a centralised platfrom for fundraising, marketing and ticketing, licensed by 750+ leading arts organisations.',
                'Ported functionality from a legacy desktop application to a single page web application backed by a RESTful API.',
                'Implemented pandemic rapid response features for the Tessitura ecommerce platform, allowing 500+ member organisations to retain revenue and stay connected with customers.',
                'Responsible for database architecture, API design and Android client implementation of an access control / ticket scanning application.'

            ],
            technologies: ['C#', 'ASP.NET', 'Angular', 'SQL Server', 'Kotlin', 'Android']
        },
        {
            title: 'Software Engineer',
            start: 'Mar 2017',
            end: 'Sep 2020',
            company: 'Tessitura Network',
            achievements: [
                'Technical lead for integrating Sisense analytics platform with Tessitura enterprise CRM.',
                'Developed single sign on solution between the two systems using ASP.NET and Angular.',
                'Built continuous integration pipelines in GoCD and implemented end to end tests with Selenium.',
                'Created custom visualisations using JavaScript, AngularJS, Highcharts, and D3.'
            ],
            technologies: ['C#', 'ASP.NET', 'Angular', 'SQL Server', 'Sisense', 'JavaScript', 'WPF']
        },
        {
            title: 'Support & Application Consultant',
            start: 'Jan 2015',
            end: 'Mar 2017',
            company: 'Tessitura Network',
            achievements: [
                'Completed 5 data conversion projects, migrating client data from legacy systems into Tessitura with a 100% success rate.',
                'Provided excellent support for users of Tessitura software, resolving impactful and time-sensitive issues with urgency.',
                'Built custom applications for clients using ASP.NET, JavaScript, SQL Server and WPF.'
            ],
            technologies: ['C#', 'ASP.NET', 'SQL Server', 'JavaScript', 'WPF', 'Azure DevOps']
        },
    ]

    const certifications: Certificate[] = [
        {
            title: 'Red Hat Certified Systems Administrator (RHCSA)',
            date: 'Mar 2023',
            url: 'https://www.credly.com/badges/de2f2882-3702-48e0-9c6e-1d0440413497'
        },
        {
            title: 'AWS Certified Solutions Architect - Professional',
            date: 'Sep 2022',
            url: 'https://www.credly.com/badges/3db5fc93-4a3e-4639-8676-2d6728ce2f31'
        },
        {
            title: 'AWS Certified Solutions Architect - Associate',
            date: 'May 2022',
            url: 'https://www.credly.com/badges/16b70a18-a010-47ff-8217-a862551830c2'
        }
    ]

    const projects: Project[] = [
        {
            title: 'Cloud Resume Challenge',
            url: 'https://github.com/pdrew/cloud-resume-challenge',
            date: 'Since 2023',
            detail: 'Resume website created with Next.js and Tailwind, hosted in S3. Visitor counter powered by Lambda, DynamoDB and DynamoDB Streams. Infrastructure deployed with AWS CDK, CI/CD managed with GitHub Actions.',
            technologies: ['AWS', 'AWS CDK', 'C#', 'ASP.NET', 'Lambda', 'DynamoDB', 'Next.js', 'GitHub Actions']
        }
    ]

    const skillCategories: SkillCategory[] = [
        {
            title: 'Proficient',
            skills: [
                'C#', 'ASP.NET', 'SQL', 'SQL Server', 'JavaScript', 'AWS', 'AWS CDK', 'PowerShell'
            ]
        },
        {
            title: 'Familiar',
            skills: [
                'Angular', 'Kotlin', 'Android', 'WPF', 'Linux', 'Git', 'GoCD', 'Bamboo', 'Azure DevOps', 'GitHub Actions', 'Bitbucket Pipelines'
            ]
        }
    ]

    return {
        positions,
        certifications,
        projects,
        skillCategories,
    }
}