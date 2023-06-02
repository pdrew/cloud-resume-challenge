import Header from '../components/header'
import Experience from '../components/experience'
import Certifications from '../components/certifications'
import Projects from '../components/projects'
import Skills from '../components/skills'
import Contact from '../components/contact'
import { getResume } from '../lib/resume'
import Counter from '../components/counter'

const url = `https://${process.env.NEXT_PUBLIC_API_DOMAIN}/views`;

fetch(url, { method: 'POST' }).then();

const timestamp = Math.floor(Date.now() / 1000);

export function getStaticProps() {
    
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

export default function Home({ positions, certifications, projects, skillCategories }) {
    return (
        <div className="p-6 mx-auto page max-w-2xl print:max-w-letter md:max-w-letter md:h-letter xsm:p-8 sm:p-9 md:p-16 bg-white">
            <Header/>
            <div className="md:col-count-2 print:col-count-2 col-gap-md md:h-letter-col print:h-letter-col col-fill-auto">
                <Experience positions={positions} />
                <Certifications certifications={certifications} />
                <Projects projects={projects} />
                <Skills categories={skillCategories} />
                <Contact />
                <Counter url={url} timestamp={timestamp} />
            </div>                     
        </div>
    )
  }