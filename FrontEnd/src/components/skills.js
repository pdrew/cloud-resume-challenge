export default function Skills({ categories }) {
    return (
        <section class="mt-8 first:mt-0">
            <div class="break-inside-avoid">
                <h2 class="mb-4 font-bold tracking-widest text-sm2 text-gray-550 print:font-normal">
                SKILLS
                </h2>
                {categories.map((category) => (
                    <section class="mb-4.5 break-inside-avoid">
                        <header>
                            <h3 class="text-lg font-semibold text-gray-700 leading-snugish">
                                {category.title}
                            </h3>
                        </header>
                        <div class="my-3.2 last:pb-1.5">
                            <ul class="flex flex-wrap text-md leading-relaxed -mr-1.6 -mb-1.6">
                                {category.skills.map((skill) => (
                                    <li class="px-2.5 mr-1.6 mb-1.6 text-base text-gray-750 print:bg-white print:border-inset bg-gray-200">
                                    {skill}
                                    </li> 
                                ))}
                            </ul>
                        </div>
                    </section>
                ))}
            </div>
        </section>
    )
}