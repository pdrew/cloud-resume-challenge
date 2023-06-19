import Position from "./position";
import Section from "./section";

export default function Experience({ positions }: { positions: Position[] }) {
    return (    
        <Section title="EXPERIENCE">
            {positions.map((p, i) => (
                <Position position={p} key={`position-${i}`} />
            ))}
        </Section>
    )
}