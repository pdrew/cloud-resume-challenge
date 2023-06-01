import Position from "./position";

export default function Experience({ positions }) {
    return (    
        <section class="mt-8 first:mt-0">
            <div class="break-inside-avoid">
                <h2 class="mb-4 font-bold tracking-widest text-sm2 text-gray-550 print:font-normal">
                    EXPERIENCE
                </h2>
                {positions.map((p) => (
                    <Position position={p} />
                ))}
            </div>
        </section>
    )
}