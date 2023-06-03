import Section from "./section";

export default function Skills({ categories }) {
    return (
        <Section title="SKILLS">
            {categories.map((category, i) => (
                <section className="mb-4.5 break-inside-avoid" key={`skill-category-${i}`}>
                    <header>
                        <h3 className="text-lg font-semibold text-gray-700 leading-snugish">
                            {category.title}
                        </h3>
                    </header>
                    <div className="my-3.2 last:pb-1.5">
                        <ul className="flex flex-wrap text-md leading-relaxed -mr-1.6 -mb-1.6">
                            {category.skills.map((skill, j) => (
                                <li className="px-2.5 mr-1.6 mb-1.6 text-base text-gray-750 print:bg-white print:border-inset bg-gray-200" key={`skill-${j}`}>
                                {skill}
                                </li> 
                            ))}
                        </ul>
                    </div>
                </section>
            ))}
        </Section>
    )
}