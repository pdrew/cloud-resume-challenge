import SectionHeader from "./sectionHeader"

export default function Section({ title, children }) {
    return (
        <section className="mt-8 first:mt-0">
            <div className="break-inside-avoid">
                <SectionHeader title={title}/>
                {children}
            </div>
        </section>
    )
}