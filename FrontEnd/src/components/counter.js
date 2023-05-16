import useSWR, { mutate } from 'swr';

const fetcher = (...args) => fetch(...args).then((res) => res.json());

export const url = `https://${process.env.NEXT_PUBLIC_API_DOMAIN}/views`;

export default function Counter() {
    const { data, error, isLoading } = useSWR(url, fetcher)
    
    if (error) return <p>Failed to load view statistics</p>
    if (isLoading) return <p>Fetching view statistics...</p>

    return <p>This page has had <span id="total-views">{data.totalViews}</span> total views by <span id="unique-visitors">{data.uniqueVisitors}</span> unique visitors.</p>
}