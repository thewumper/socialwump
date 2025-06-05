import { produce } from 'sveltekit-sse';

function delay(milliseconds: number) {
	return new Promise(function run(resolve) {
		setTimeout(resolve, milliseconds);
	});
}

async function getEvents(lastEvent: number) {
	// Testhing things for atticus
	const events = await fetch('http://wumpapi:8080/events', {
		method: 'POST',
		body: JSON.stringify({
			LastEvent: lastEvent
		}),
		headers: {
			'Content-Type': 'application/json'
		}
	});
	const body = await events.json();
	return body;
}

export function POST() {
	return produce(async function start({ emit }) {
		const allEvents = await getEvents(0);
		const lastEvent = allEvents.slice(-1)[0];
		let lastEventNumber = lastEvent.initiatedAt;

		while (true) {
			await delay(1000);

			const newestEvents = await getEvents(lastEventNumber + 1);
			if (newestEvents.length === 0) {
				continue;
			} else {
				newestEvents.forEach((event) => {
					console.log(event);
					lastEventNumber = event.initiatedAt;
					const { error } = emit(event.name, JSON.stringify(event));
					if (error) {
						return;
					}
				});
			}
		}
	});
}
