export default function Position({ position }) {
    return (
        <section class="mb-4.5 break-inside-avoid">
            <header>
                <h3 class="text-lg font-semibold text-gray-700 leading-snugish">
                    {position.title}
                </h3>
                <p class="leading-normal text-md text-gray-650">
                    {position.start} - {position.end} | {position.company}
                </p>
            </header>
            <ul class="">
                {position.achievements.map((achievement) => (
                    <li class="mt-2.1 text-md text-gray-700 leading-normal">
                        <span class="absolute -ml-3 sm:-ml-3.2 select-none transform -translate-y-px">›</span>
                        {achievement}
                    </li>
                ))}
            </ul>
        </section>
    )
}