import Head from 'next/head';

const name = 'Patrick Drew';
export const siteTitle = 'Patrick Drew Resume';

export default function Layout({ children }) {
  return (
    <main className="font-firago hyphens-manual">
      <Head>
        <meta name="robots" content="noindex" />
        <meta name="og:title" content={siteTitle} />
        <meta name="twitter:card" content="summary_large_image" />
      </Head>
      {children}
    </main>
  );
}