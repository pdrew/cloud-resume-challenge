import useSWR, { mutate } from 'swr';

const fetcher = (...args) => fetch(...args).then((res) => res.json());

export const url = 'https://2azrjwirb3.execute-api.ap-southeast-2.amazonaws.com/prod/views';

export default function Counter() {
    const { data, error, isLoading } = useSWR(url, fetcher)
    
    if (error) return <p>Failed to load view statistics</p>
    if (isLoading) return <p>Fetching view statistics...</p>

    return <p>This page has been viewed {data.total} times.</p>
}