import useSWR from 'swr';

const fetcher = (...args) => fetch(...args).then((res) => res.json());

function ErrorMessage() {
    return (
        <section className="mt-8 first:mt-0">
          <p className="leading-normal text-md text-gray-650">Failed to load view statistics.</p>
        </section>
    )
}

function LoadingMessage() {
    return (
        <section className="mt-8 first:mt-0">
          <p className="leading-normal text-md text-gray-650">Fetching view statistics...</p>
        </section>
    )
}

function SuccessMessage(data) {
    return (
        <section className="mt-8 first:mt-0">
          <p className="leading-normal text-md text-gray-650">This page has been viewed <span id="total-views">{data.totalViews}</span> times by <span id="unique-visitors">{data.uniqueVisitors}</span> unique visitors in the past month.</p>
        </section>
    )
}

export default function Counter({ url, timestamp }) {
    const { data, error, isLoading } = useSWR(`${url}?timestamp=${timestamp}`, fetcher)
    
    if (error) return ErrorMessage();
    if (isLoading) return LoadingMessage();

    return SuccessMessage(data);
}