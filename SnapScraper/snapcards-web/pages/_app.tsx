import type { AppProps } from 'next/app'
import '../styles/index.css'

export default function MyApp({ Component, pageProps }: AppProps) {
  return (
    <div className="bg-dudco-green">
      <Component {...pageProps} />
    </div>
  );
}
