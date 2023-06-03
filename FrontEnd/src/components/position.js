export default function Position({ position }) {
    return (
        <section className="mb-4.5 break-inside-avoid">
            <header>
                <h3 className="text-lg font-semibold text-gray-700 leading-snugish">
                    {position.title}
                </h3>
                <p className="leading-normal text-md text-gray-650">
                    {position.start} - {position.end} | {position.company}
                </p>
            </header>
            <ul className="">
                {position.achievements.map((achievement, i) => (
                    <li className="mt-2.1 text-md text-gray-700 leading-normal" key={`achievement-${i}`}>
                        <span className="absolute -ml-3 sm:-ml-3.2 select-none transform -translate-y-px">›</span>
                        {achievement}
                    </li>
                ))}
            </ul>
        </section>
    )
}