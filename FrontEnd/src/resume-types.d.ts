interface Position {
    title: string,
    start: string,
    end: string,
    company: string,
    achievements: string[],
    technologies: string[]
}

interface Certificate {
    title: string,
    date: string,
    url: string
}

interface Project {
    title: string,
    url: string,
    date: string,
    detail: string,
    technologies: string[]
}

interface SkillCategory {
    title: string,
    skills: string[]
}

interface Resume {
    positions: Position[],
    certifications: Certificate[],
    projects: Project[],
    skillCategories: SkillCategory[]
}