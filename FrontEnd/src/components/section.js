import SectionHeader from "./sectionHeader"

export default function Section({ title, children }) {
    return (
        <section class="mt-8 first:mt-0">
            <div class="break-inside-avoid">
                <SectionHeader title={title}/>
                {children}
            </div>
        </section>
    )
}