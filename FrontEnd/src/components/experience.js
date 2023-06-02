import Position from "./position";
import Section from "./section";

export default function Experience({ positions }) {
    return (    
        <Section title="EXPERIENCE">
            {positions.map((p) => (
                <Position position={p} />
            ))}
        </Section>
    )
}