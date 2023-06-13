import Section from "./section";

export default function Projects({ projects }: { projects: Project[] }) {
    return (
        <Section title="PROJECTS">
            {projects.map((project, i) => (
                <section className="mb-4.5 break-inside-avoid" key={`project-${i}`}>
                    <header>
                        <h3 className="text-lg font-semibold text-gray-700 leading-snugish">
                        <a href={project.url} className="group">
                            {project.title}
                            <span className="inline-block text-gray-550 print:text-black font-normal group-hover:text-gray-700 transition duration-100 ease-in">↗</span>
                        </a>
                        </h3>
                        <p className="leading-normal text-md text-gray-650">
                        {project.date} | {project.technology}
                        </p>
                    </header>
                    <p className="mt-2.1 text-md text-gray-700 leading-normal">
                        {project.detail}
                    </p>
                </section> 
            ))}
        </Section>
    )
}