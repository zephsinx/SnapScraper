# Snapcards

## How to use

Open a terminal/command prompt to the root of the `snapcards-web` folder and run the following commands, waiting for
each to finish before running the next one:

```shell
node install
npm run build
npm run start
```

You should see a message that says `Local: http://localhost:3000`. Go to that URL and you should see the grid after a
few seconds.

Additionally, you will need to update the `config.ts` file with the Action IDs from the Streamer.bot actions you want to
run. Look for `ACTION_1_ID` and `ACTION_2_ID`.
