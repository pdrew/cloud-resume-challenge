import Header from '../components/header'
import Experience from '../components/experience'
import Certifications from '../components/certifications'
import Projects from '../components/projects'
import { getResume } from '../lib/resume'
import Counter from '../components/counter'
import Column from '../components/column'
import Page from '../components/page'
import Layout from '../components/layout'
import { GetStaticProps } from 'next'

const baseUrl = `https://${process.env.NEXT_PUBLIC_API_DOMAIN}`;

fetch(`${baseUrl}/visitors`, { method: 'POST' }).then();

const timestamp = Math.floor(Date.now() / 1000);

export const getStaticProps: GetStaticProps = async (context) => {
    
    const resume = getResume();

    return {
        props: {
            positions: resume.positions,
            certifications: resume.certifications,
            projects: resume.projects
        }
    }
}

export default function Home({ positions, certifications, projects }: {
    positions: Position[],
    certifications: Certificate[],
    projects: Project[],
    skillCategories: SkillCategory[]
}) {
    return (
        <Layout>
            <Page>
                <Header/>
                <Column>
                    <Experience positions={positions} />
                    <Certifications certifications={certifications} />
                    <Projects projects={projects} />
                    <Counter baseUrl={baseUrl} timestamp={timestamp} />
                </Column>
            </Page>
        </Layout>
    )
}