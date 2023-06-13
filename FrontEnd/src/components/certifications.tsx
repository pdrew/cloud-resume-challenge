import Section from "./section"

export default function Certifications({certifications}: {certifications: Certificate[]}) {
    return (
        <Section title="CERTIFICATIONS">
            {certifications.map((certificate, i) => (
                <section className="mb-4.5 break-inside-avoid" key={`certification-${i}`}>
                    <header>
                        <h3 className="text-lg font-semibold text-gray-700 leading-snugish">
                        {certificate.title}
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