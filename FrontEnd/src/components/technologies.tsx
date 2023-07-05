export default function Technologies({ technologies }: {technologies: string[]}) {
    return (
        <div className="my-3.2 last:pb-1.5">
            <ul className="flex flex-wrap text-md leading-relaxed -mr-1.6 -mb-1.6">
                {technologies.map((technology, j) => (
                    <li className="px-2.5 mr-1.6 mb-1.6 text-base text-gray-750 print:bg-white print:border-inset bg-gray-200" key={`skill-${j}`}>
                    {technology}
                    </li> 
                ))}
            </ul>
        </div>
    )
}