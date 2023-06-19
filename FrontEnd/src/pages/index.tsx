import Header from '../components/header'
import Experience from '../components/experience'
import Certifications from '../components/certifications'
import Projects from '../components/projects'
import Skills from '../components/skills'
import Contact from '../components/contact'
import { getResume } from '../lib/resume'
import Counter from '../components/counter'
import Column from '../components/column'
import Page from '../components/page'
import Layout from '../components/layout'
import { GetStaticProps } from 'next'

const url = `https://${process.env.NEXT_PUBLIC_API_DOMAIN}/views`;

fetch(url, { method: 'POST' }).then();

const timestamp = Math.floor(Date.now() / 1000);

export const getStaticProps: GetStaticProps = async (context) => {
    
    const resume = getResume();

    return {
        props: {
            positions: resume.positions,
            certifications: resume.certifications,
            projects: resume.projects,
            skillCategories: resume.skillCategories
        }
    }
}

export default function Home({ positions, certifications, projects, skillCategories }: {
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
                    <Skills categories={skillCategories} />
                    <Contact />
                    <Counter url={url} timestamp={timestamp} />
                </Column>
            </Page>
        </Layout>
    )
}