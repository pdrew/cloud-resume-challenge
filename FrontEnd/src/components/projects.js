export default function Projects({ projects }) {
    return (
        <section class="mt-8 first:mt-0">
          <div class="break-inside-avoid">

            <h2 class="mb-4 font-bold tracking-widest text-sm2 text-gray-550 print:font-normal">
              PROJECTS
            </h2>

            {projects.map((project) => (
                <section class="mb-4.5 break-inside-avoid">
                    <header>
                        <h3 class="text-lg font-semibold text-gray-700 leading-snugish">
                        <a href={project.url} class="group">
                            {project.title}
                            <span class="inline-block text-gray-550 print:text-black font-normal group-hover:text-gray-700 transition duration-100 ease-in">↗</span>
                        </a>
                        </h3>
                        <p class="leading-normal text-md text-gray-650">
                        {project.date} | {project.technology}
                        </p>
                    </header>
                    <p class="mt-2.1 text-md text-gray-700 leading-normal">
                        {project.detail}
                    </p>
                </section> 
            ))}

          </div>

        </section>
    )
}