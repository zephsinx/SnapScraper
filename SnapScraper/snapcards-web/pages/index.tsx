import type { NextPage } from "next";
import Head from "next/head";
import Image from "next/image";
import { Card } from "../utils/types";
import config from "../utils/config";
import React, { useState } from "react";
import { StreamerbotClient } from "@streamerbot/client";

const Home: NextPage = ({ cards }: { cards: Card[] }) => {
  const [searchQuery, setSearchQuery] = useState("");
  // Create a new client with default options
  const client = new StreamerbotClient();
  const action1Id = config.ACTION_1_ID;
  const action2Id = config.ACTION_2_ID;

  return (
    <>
      <Head>
        <title>SNAP</title>
      </Head>
      <main className="mx-auto max-w-[1960px] p-4">
        <div className="search-bar mx-auto w-full max-w-md">
          <input
            type="text"
            placeholder="Search..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="mt-2 w-full rounded-lg border border-gray-300 bg-gray-100 px-4 py-2 text-gray-800 focus:border-blue-500 focus:outline-none"
          />
        </div>

        <div className="grid gap-4 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6">
          {cards
            .filter((card) =>
              card.name.toLowerCase().includes(searchQuery.toLowerCase())
            )
            .map(
              ({
                art,
                name,
                carddefid,
                ability,
                cost,
                power,
                flavor,
                difficulty,
                rarity,
                source,
                status,
                type,
                url,
              }) => (
                <div key={carddefid}>
                  <Image
                    alt={name}
                    className="transform rounded-lg brightness-90 transition will-change-auto group-hover:brightness-110"
                    style={{ transform: "translate3d(0, 0, 0)" }}
                    src={art}
                    width={512}
                    height={512}
                    sizes="(max-width: 512px) 100vw"
                  />
                  <div className="flex items-center justify-center font-bold">
                    <text>{name}</text>
                  </div>
                  <div className="button-container flex items-center justify-center">
                    <button
                      onClick={() =>
                        client.doAction(action1Id, {
                          cardArtUrl: art,
                          cardName: name,
                          cardAbility: ability,
                          cardCost: cost,
                          cardPower: power,
                          cardFlavor: flavor,
                          cardDifficulty: difficulty,
                          cardRarity: rarity,
                          cardSource: source,
                          cardStatus: status,
                          cardType: type,
                        })
                      }
                      className="m-1 rounded bg-blue-500 px-2 py-1 font-bold text-white hover:bg-blue-700"
                    >
                      Action 1
                    </button>
                    <button
                      onClick={() =>
                        client.doAction(action2Id, {
                          cardArtUrl: art,
                          cardName: name,
                          cardAbility: ability,
                          cardCost: cost,
                          cardPower: power,
                          cardFlavor: flavor,
                          cardDifficulty: difficulty,
                          cardRarity: rarity,
                          cardSource: source,
                          cardStatus: status,
                          cardType: type,
                        })
                      }
                      className="m-1 rounded bg-blue-500 px-2 py-1 font-bold text-white hover:bg-blue-700"
                    >
                      Action 2
                    </button>
                  </div>
                </div>
              )
            )}
        </div>
      </main>
    </>
  );
};

export default Home;

export async function getStaticProps() {
  const snapUrl =
    config.SNAP_URL ||
    "https://marvelsnapzone.com/getinfo/?searchtype=cards&searchcardstype=true";

  const results = await fetch(snapUrl);
  const jsonData = await results.json();
  const cards: Card[] = jsonData.success.cards;

  return {
    props: {
      cards: cards,
    },
  };
}
