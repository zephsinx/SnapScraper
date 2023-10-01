# Snapcards

## What it is

- A grid of Marvel Snap cards with buttons to trigger actions in Streamer.bot.
- It has a search bar that you can use to filter the cards down by name.
- The two buttons under each card can be configured to trigger different streamer.bot actions.

Aside from triggering the action, it'll set the following variables in Streamer.bot to be used however needed:

```text
%cardArtUrl%
%cardName%
%cardAbility%
%cardCost%
%cardPower%
%cardFlavor%
%cardDifficulty%
%cardRarity%
%cardSource%
%cardStatus%
%cardType%
```

## Initial setup

1. Download and unzip the `snapcards-web.zip` file
   from https://github.com/zephsinx/SnapScraper/releases/tag/v1.0.0-snapcards
2. Open Streamer.bot and find/create one or two actions you want to trigger from the page
3. Open the `config.ts` file in the `utils` folder
4. In Streamer.bot, right-click the first action you want to assign and click `Copy Action Id`
5. Paste it into the value of `ACTION_1_ID`. (e.g. `ACTION_1_ID: 'e4e03f5c-e749-4852-9ec0-4afa7a48b37e'`)
6. Do the same with the second action you want to hook up.

## Installation

1. Make sure you have Node.js installed and updated.
2. Open a command prompt to the folder you unzipped (`snapcards-web`) and run the following commands, waiting for each
   to finish:

   ```shell
   npm install
   npm run build
   ```

## Running the app

1. From the `snapcards-web` folder, run the following command:

   ```shell
   npm run start
   ```

2. The app should start up and be accessible at http://localhost:3000/
