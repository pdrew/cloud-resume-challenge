import Head from 'next/head';

const name = 'Patrick Drew';
export const siteTitle = 'Patrick Drew Resume';

export default function Layout({ children, home }) {
  return (
    <main class="font-firago hyphens-manual">
      <Head>
        <meta name="og:title" content={siteTitle} />
        <meta name="twitter:card" content="summary_large_image" />
      </Head>
      {children}
    </main>
  );
}