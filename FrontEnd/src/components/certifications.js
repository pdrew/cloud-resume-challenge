import Section from "./section"

export default function Certifications({ certifications }) {
    return (
        <Section title="CERTIFICATIONS">
            {certifications.map((certificate) => (
                <section class="mb-4.5 break-inside-avoid">
                    <header>
                        <h3 class="text-lg font-semibold text-gray-700 leading-snugish">
                        {certificate.title}
                        </h3>
                        <p class="leading-normal text-md text-gray-650">
                        {certificate.date}
                        </p>
                    </header>
                </section>
            ))}  
        </Section>
    )
}