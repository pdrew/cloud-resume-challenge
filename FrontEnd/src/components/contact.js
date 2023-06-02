import Section from "./section";

export default function Contact() {
    return (
        <Section title="CONTACT">
            <section class="mb-4.5 break-inside-avoid">
              <ul class="list-inside pr-7">
                <li class="mt-1.5 leading-normal text-gray-700 text-md">
                  <a href="https://www.linkedin.com/in/patrick-drew-41493432" class="group">
                    LinkedIn
                    <span class="inline-block text-gray-550 print:text-black font-normal group-hover:text-gray-700 transition duration-100 ease-in">↗</span>
                  </a>
                </li>
                <li class="mt-1.5 leading-normal text-gray-700 text-md">
                  patrick.r.drew@gmail.com
                </li>
              </ul>
            </section>
        </Section>
    )
}