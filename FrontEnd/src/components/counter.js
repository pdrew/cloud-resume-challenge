import useSWR from 'swr';

const fetcher = (...args) => fetch(...args).then((res) => res.json());

export default function Counter({ url, timestamp }) {
    const { data, error, isLoading } = useSWR(`${url}?timestamp=${timestamp}`, fetcher)
    
    if (error) return <p>Failed to load view statistics</p>
    if (isLoading) return <p>Fetching view statistics...</p>

    return <p>This page has been viewed <span id="total-views">{data.totalViews}</span> times by <span id="unique-visitors">{data.uniqueVisitors}</span> unique visitors in the past month.</p>
}