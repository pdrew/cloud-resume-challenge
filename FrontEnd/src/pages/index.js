import Header from '../components/header'
import Experience from '../components/experience'
import Certifications from '../components/certifications'
import Projects from '../components/projects'
import Skills from '../components/skills'
import Contact from '../components/contact'

export function getStaticProps() {
    const positions = [
        {
            title: 'Infrastructure Engineer',
            start: 'Apr 2022',
            end: 'Present',
            company: 'Tessitura',
            achievements: [
                'Cooked shrimps for 2 to 3 minutes per side, or until opaque; then, transfered to a serving dish with limon',
                'Roasted a pig, turning frequently, until meat reached 160°F in the thickest part of the shoulder or thigh',
                'Filled burgdoggen & frankfurter strip steak with 90% burger patties and broth'
            ]
        },
        {
            title: 'Senior Software Engineer',
            start: 'Sep 2020',
            end: 'Apr 2022',
            company: 'Tessitura',
            achievements: [
                'Cooked shrimps for 2 to 3 minutes per side, or until opaque; then, transfered to a serving dish with limon',
                'Roasted a pig, turning frequently, until meat reached 160°F in the thickest part of the shoulder or thigh',
                'Filled burgdoggen & frankfurter strip steak with 90% burger patties and broth'
            ]
        },
        {
            title: 'Software Engineer',
            start: 'Mar 2017',
            end: 'Sep 2020',
            company: 'Tessitura',
            achievements: [
                'Cooked shrimps for 2 to 3 minutes per side, or until opaque; then, transfered to a serving dish with limon',
                'Roasted a pig, turning frequently, until meat reached 160°F in the thickest part of the shoulder or thigh',
                'Filled burgdoggen & frankfurter strip steak with 90% burger patties and broth'
            ]
        },
        {
            title: 'Support & Application Consultant',
            start: 'Jan 2015',
            end: 'Mar 2017',
            company: 'Tessitura',
            achievements: [
                'Cooked shrimps for 2 to 3 minutes per side, or until opaque; then, transfered to a serving dish with limon',
                'Roasted a pig, turning frequently, until meat reached 160°F in the thickest part of the shoulder or thigh',
                'Filled burgdoggen & frankfurter strip steak with 90% burger patties and broth'
            ]
        },
    ]

    const certifications = [
        {
            title: 'Red Hat Certified Systems Administrator (RHCSA)',
            date: 'Mar 2023'
        },
        {
            title: 'AWS Certified Solutions Architect - Professional',
            date: 'Sep 2022'
        },
        {
            title: 'AWS Certified Solutions Architect - Associate',
            date: 'May 2022'
        }
    ]

    const projects = [
        {
            title: 'Cloud Resume Challenge',
            url: 'https://github.com/pdrew/cloud-resume-challenge',
            date: 'Since 2023',
            technology: 'C# AWS',
            detail: 'Good design is as little design as possible. Less, but better because it concentrates on the essential aspects, and the products are not burdened with non-essentials.'
        }
    ]

    const skillCategories = [
        {
            title: 'Proficient',
            skills: [
                'C#', 'ASP.NET', 'SQL', 'SQL Server', 'JavaScript', 'AWS', 'AWS CDK', 'PowerShell'
            ]
        },
        {
            title: 'Familiar',
            skills: [
                'Angular', 'Kotlin', 'Android', 'WPF', 'Linux', 'Git', 'GoCD', 'Bamboo', 'Azure DevOps', 'GitHib Actions', 'Bitbucket Pipelines'
            ]
        }
    ]

    return {
        props: {
            positions,
            certifications,
            projects,
            skillCategories,
        }
    }
}

export default function Home({ positions, certifications, projects, skillCategories }) {
    return (
        <div class="p-6 mx-auto page max-w-2xl print:max-w-letter md:max-w-letter md:h-letter xsm:p-8 sm:p-9 md:p-16 bg-white">
            <Header/>
            <div class="md:col-count-2 print:col-count-2 col-gap-md md:h-letter-col print:h-letter-col col-fill-auto">
                <Experience positions={positions} />
                <Certifications certifications={certifications} />
                <Projects projects={projects} />
                <Skills categories={skillCategories} />
                <Contact />
            </div>                     
        </div>
    )
  }