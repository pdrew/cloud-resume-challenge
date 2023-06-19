import Section from "./section"

export default function Certifications({certifications}: {certifications: Certificate[]}) {
    return (
        <Section title="CERTIFICATIONS">
            {certifications.map((certificate, i) => (
                <section className="mb-4.5 break-inside-avoid" key={`certification-${i}`}>
                    <header>
                        <h3 className="text-lg font-semibold text-gray-700 leading-snugish">
                        <a href={certificate.url} className="group" target="_blank" rel="noopener noreferrer">
                            {certificate.title}
                            <span className="inline-block text-gray-550 print:text-black font-normal group-hover:text-gray-700 transition duration-100 ease-in">↗</span>
                        </a>
                        </h3>
                        <p className="leading-normal text-md text-gray-650">
                        {certificate.date}
                        </p>
                    </header>
                </section>
            ))}  
        </Section>
    )
}