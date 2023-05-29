import Head from 'next/head';
import Layout, { siteTitle } from '../components/layout';
import utilStyles from '../styles/utils.module.css';
import Counter from '../components/counter';

const url = `https://${process.env.NEXT_PUBLIC_API_DOMAIN}/views`;

const fetcher = (...args) => fetch(...args).then((res) => res.json());
fetcher(url, { method: 'POST' }).then();

const timestamp = Math.floor(Date.now() / 1000);

export default function Home() {

  return (
    <Layout home>
      <Head>
        <title>{siteTitle}</title>
      </Head>
      <section className={utilStyles.headingMd}>
        <p>Hi! I'm Patrick Drew, <span>Software Engineer at <a href="https://www.tessituranetwork.com/">Tessitura Network.</a></span> You can contact me on <span><a href="https://www.linkedin.com/in/patrick-drew-41493432/">LinkedIn.</a></span></p>
        <p>I created this page as part of the <span><a href="https://cloudresumechallenge.dev">Cloud Resume Challenge.</a></span></p>
        <Counter url={url} timestamp={timestamp}/>
      </section>
      <section className={`${utilStyles.headingMd} ${utilStyles.padding1px}`}>
        <h2 className={utilStyles.headingLg}>Work Experience</h2>
        <ul className={utilStyles.list}>
        <li className={utilStyles.listItem}>
            <strong>Infrastructure Engineer</strong>
            <br />
            <small className={utilStyles.lightText}>Tessitura Network <span>•</span> April 2022 - Present</small>
          </li>
          <li className={utilStyles.listItem}>
            <strong>Senior Software Engineer</strong>
            <br />
            <small className={utilStyles.lightText}>Tessitura Network <span>•</span> September 2020 - April 2022</small>
          </li>
          <li className={utilStyles.listItem}>
            <strong>Software Engineer</strong>
            <br />
            <small className={utilStyles.lightText}>Tessitura Network <span>•</span> March 2017 - September 2020</small>
          </li>
          <li className={utilStyles.listItem}>
            <strong>Support & Application Consultant</strong>
            <br />
            <small className={utilStyles.lightText}>Tessitura Network <span>•</span> January 2015 - March 2017</small>           
          </li>
        </ul>
      </section>
      <section className={`${utilStyles.headingMd} ${utilStyles.padding1px}`}>
        <h2 className={utilStyles.headingLg}>Certifications</h2>
        <ul className={utilStyles.list}>
          <li className={utilStyles.listItem}>
            <strong>Red Hat Certified Systems Administrator (RHCSA)</strong>
            <br />
            <small className={utilStyles.lightText}>March 2023</small>
          </li>
          <li className={utilStyles.listItem}>
            <strong>AWS Certified Solutions Architect - Professional</strong>
            <br />
            <small className={utilStyles.lightText}>September 2022</small>
          </li>
          <li className={utilStyles.listItem}>
            <strong>AWS Certified Solutions Architect - Associate</strong>
            <br />
            <small className={utilStyles.lightText}>May 2022</small>
          </li>
        </ul>
      </section>
    </Layout>
  );
}