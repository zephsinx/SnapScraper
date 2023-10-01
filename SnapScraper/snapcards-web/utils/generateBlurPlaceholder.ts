import imagemin from 'imagemin'
import imageminJpegtran from 'imagemin-jpegtran'
import {Card} from "./types";

const cache = new Map<Card, string>();

export default async function getBase64ImageUrl(card: Card): Promise<string> {
    try {
        let url = cache.get(card)
        if (url) {
            return url;
        }
        const response = await fetch(card.art);
        const buffer = await response.arrayBuffer();
        const minified = await imagemin.buffer(Buffer.from(buffer), {
            plugins: [imageminJpegtran()],
        });

        url = `data:image/jpeg;base64,${Buffer.from(minified).toString('base64')}`;
        cache.set(card, url);
        return url;
    } catch {
        console.log(`Error when generating blur placeholder for carddefid '${card.carddefid}'`)
        return '';
    }
}