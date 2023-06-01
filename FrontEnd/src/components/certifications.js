export default function Certifications({ certifications }) {
    return (
        <section class="mt-8 first:mt-0">
            <div class="break-inside-avoid">
                <h2 class="mb-4 font-bold tracking-widest text-sm2 text-gray-550 print:font-normal">
                CERTIFICATIONS
                </h2>
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
            </div>
        </section>
    )
}