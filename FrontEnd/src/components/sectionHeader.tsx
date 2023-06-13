export default function SectionHeader({title}: {title: string}) {
    return (
        <h2 className="mb-4 font-bold tracking-widest text-sm2 text-gray-550 print:font-normal">
            {title}
        </h2>
    )
}