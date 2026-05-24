import signalR from "@microsoft/signalr";
import { exec } from "child_process";

const printerStates = {};

const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5053/printerHub")
  .withAutomaticReconnect()
  .build();

connection.on("PrinterStatusUpdate", (printers) => {
  const now = Date.now();

  printers.forEach((printer) => {
    const name = printer.printerName;

    if (!printerStates[name]) {
      printerStates[name] = {
        lastJobs: printer.jobs,
        lastChange: now,
        stuck: false,
      };
    }

    const state = printerStates[name];

    // queue changed
    if (printer.jobs !== state.lastJobs) {
      state.lastJobs = printer.jobs;
      state.lastChange = now;
      state.stuck = false;
      return;
    }

    const isNowStuck = printer.jobs > 0 && now - state.lastChange > 10000;

    // trigger terminal once
    if (isNowStuck && !state.stuck) {
      exec(`start cmd /k echo [STUCK] ${name} queue frozen`);

      console.log(`[STUCK] ${name}`);
    }

    state.stuck = isNowStuck;
  });
});

async function start() {
  try {
    await connection.start();
    console.log("Connected.");
  } catch (err) {
    console.error(err);
    setTimeout(start, 2000);
  }
}

start();
