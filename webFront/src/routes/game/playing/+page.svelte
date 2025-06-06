<script lang="ts">
	import { onMount } from 'svelte';
	import * as d3 from 'd3';
	import { fade } from 'svelte/transition';
	import Powerbar from '$lib/components/Powerbar.svelte';
	import { enhance } from '$app/forms';
	import ItemBar from '$lib/components/ItemBar.svelte';
	import { source } from 'sveltekit-sse';

	import {
		ArrowBigDown,
		ArrowBigUp,
		Zap,
		Clock,
		Hammer,
		ArrowBigLeft,
		ArrowBigRight
	} from '@lucide/svelte';

	let errored = $state(false);
	let selectedNode = $state(null);
	let shopOpen = $state(false);
	let shopPage = $state(0);
	let hasPlayerJoinedGame = $state(false);
	let crosshairContainer: d3.Selection<SVGGElement, unknown, HTMLElement, any> | null = null;
	let svgElement;
	let simulation;
	let simulationData;
	let screenWidth;
	let screenHeight;
	let nodes;
	let links;
	let mainGroup;
	let linksGroup;
	let nodesGroup;

	let minimizeJoinGameButtonBox = $state();
	let joinGameModalOpen = $state(true);

	const { data: PageData } = $props();

	hasPlayerJoinedGame = PageData.playerJoined;

	const userPowerLevel = $state({ power: 5 });
	const playerJoin = source('/game/events/').select('GraphAddNodeEvent');

	playerJoin.subscribe((playerJoin) => {
		if (playerJoin !== '') {
			const objectObjectObject = JSON.parse(playerJoin);
			console.log(objectObjectObject);

			const newNode = {
				id: objectObjectObject.node.id,
				player: objectObjectObject.node.player
			};
			simulationData.nodes = [...simulationData.nodes, newNode];
			// Re-render and update simulation
			renderGraph();
			updateSimulation();
		}
	});

	// Change the crosshair display mode when the selected node state changes
	$effect(() => {
		const _ = selectedNode; // Variable needs to be accessed for the effect to run
		if (!crosshairContainer) return;

		if (selectedNode) {
			crosshairContainer.style('display', 'block');
		} else {
			crosshairContainer.style('display', 'none');
		}
	});

	const drag = d3
		.drag()
		.on('start', function (event, d) {
			if (!event.active) simulation.alphaTarget(0.3).restart();
			d.fx = event.x;
			d.fy = event.y;
		})
		.on('drag', function (event, d) {
			d.fx = event.x;
			d.fy = event.y;
		})
		.on('end', function (event, d) {
			if (!event.active) simulation.alphaTarget(0);
			d.fx = null;
			d.fy = null;
		});

	const zoom = d3.zoom().on('zoom', (e) => {
		mainGroup.attr('transform', e.transform);
	});

	function setupSimulation() {
		// Force simulation that does the actual layout of the graph
		simulation = d3
			.forceSimulation(simulationData.nodes)
			.force(
				'link',
				d3
					.forceLink()
					.id((d) => d.id)
					.links(simulationData.links)
			)
			.force('charge', d3.forceManyBody().strength(-200))
			.force('center', d3.forceCenter(screenWidth / 2, screenHeight / 2))
			.on('tick', ticked);
	}

	function ticked() {
		links
			.attr('x1', (d) => d.source.x)
			.attr('y1', (d) => d.source.y)
			.attr('x2', (d) => d.target.x)
			.attr('y2', (d) => d.target.y);

		nodes.attr('cx', (d) => d.x).attr('cy', (d) => d.y);

		if (selectedNode) {
			console.log('NODE');
			console.log(selectedNode.id);
			// Pull the current position data from our normal dataset to set the position of the crosshair
			const nodeData = simulationData.nodes.find((n) => {
				console.log(n);
				return n.id === selectedNode.id;
			});
			console.log(nodeData);
			crosshairContainer
				.attr('transform', `translate(${nodeData.x}, ${nodeData.y}) scale(2.25)`)
				.style('display', 'block')
				.raise();
		}
	}

	function setupCrosshair() {
		crosshairContainer = mainGroup
			.append('g')
			.attr('class', 'crosshair')
			.attr('transform', `translate(0, 0) scale(2.25)`)
			.style('display', 'none');

		crosshairContainer
			.append('path')
			.attr('class', 'crosshair')
			.attr('fill', 'white')
			.attr(
				'd',
				`M 22.2448,39.5833L 19,39.5833L 19,36.4167L 22.2448,36.4167C 22.9875,28.9363 28.9363,22.9875 36.4167,22.2448L 36.4167,19L 39.5833,19L 39.5833,22.2448C 47.0637,22.9875 53.0125,28.9363 53.7552,36.4167L 57,36.4167L 57,39.5833L 53.7552,39.5833C 53.0125,47.0637 47.0637,53.0125 39.5833,53.7552L 39.5833,57L 36.4167,57L 36.4167,53.7552C 28.9363,53.0125 22.9875,47.0637 22.2448,39.5833 Z M 25.4313,36.4167L 28.5,36.4167L 28.5,39.5833L 25.4313,39.5833C 26.1458,45.313 30.687,49.8542 36.4167,50.5687L 36.4167,47.5L 39.5833,47.5L 39.5833,50.5687C 45.313,49.8542 49.8542,45.313 50.5686,39.5833L 47.5,39.5833L 47.5,36.4167L 50.5686,36.4167C 49.8542,30.687 45.313,26.1458 39.5833,25.4314L 39.5833,28.5L 36.4167,28.5L 36.4167,25.4314C 30.687,26.1458 26.1458,30.687 25.4313,36.4167 Z `
			)
			// This transform is based on the size of the SVG so needs to be changed/removed if the crosshair is changed
			.attr('transform', 'translate(-38, -38)');
	}

	async function loadDataFromServer() {
		try {
			// Load data with promises
			simulationData = await d3.json('/game/graph');
			console.log(simulationData);
			if (simulationData == undefined) {
				errored = true;
				return;
			}
		} catch (error) {
			console.error('Error loading data:', error);
			errored = true;
			return;
		}
	}

	function updateSimulation() {
		// Update the simulation forces with new data
		simulation.nodes(simulationData.nodes);
		simulation.force('link').links(simulationData.links);

		// Restart the simulation with a lower alpha to make it converge faster
		simulation.alpha(0.3).restart();
	}

	function initializeGraph() {
		linksGroup = mainGroup.append('g').attr('class', 'links');
		nodesGroup = mainGroup.append('g').attr('class', 'nodes');
	}

	function renderGraph() {
		// Create links
		links = linksGroup
			.selectAll('line')
			.data(simulationData.links)
			.join('line')
			.attr('class', 'node')
			.style('stroke', '#aaa');

		// Create nodes
		nodes = nodesGroup
			.selectAll('circle')
			.data(simulationData.nodes)
			.join('circle')
			.attr('class', 'node')
			.attr('r', 20)
			.style('fill', '#69b3a2')
			.on('mouseover', function (event) {
				let me = d3.select(this);

				me.transition().duration(50).style('fill', '#00796B');
			})
			.on('mouseout', function (d, i) {
				d3.select(this).transition().duration(50).style('fill', '#69b3a2');

				// div.transition().duration(50).style('opacity', 0);
			})
			.on('click', function (event, d) {
				selectedNode = d;
			})
			.call(drag);

		let svg = d3.select(svgElement);
		svg.call(zoom);
	}

	function joinGame() {
		hasPlayerJoinedGame = true;

		fetch('/game/waiting/joinGame', {
			method: 'POST'
		});
	}

	onMount(async () => {
		mainGroup = d3.select(svgElement).append('g');

		screenHeight = window.innerHeight;
		screenWidth = window.innerWidth;
		setupCrosshair();
		await loadDataFromServer();
		initializeGraph();
		renderGraph();
		setupSimulation();
	});
</script>

<div class="wrapper overflow-hidden">
	{#if errored}
		<div class="centerStuffPlease">
			<h1 class="errorText">Thigns have exploded :(</h1>
		</div>
	{/if}

	{#if hasPlayerJoinedGame}
		<div class="pointer-events-none absolute h-dvh w-dvw">
			<div
				class="pointer-events-none absolute bottom-2 left-1/2 h-10 w-11/12 max-w-4xl"
				style="transform: translate(-50%,0);"
			>
				<Powerbar maxhealth="10" powerlevel={userPowerLevel.power} showNumber={true} />
			</div>
			<div
				class="pointer-events-auto absolute bottom-1/2 left-2"
				style="transform: translate(0,50%);"
			>
				<ItemBar />
			</div>
			<div
				class="pointer-events-auto absolute left-1/2 flex flex-col items-center transition-all"
				style="transform: translate(-50%,{shopOpen ? '0' : '-80dvh'});"
			>
				<div
					class="relative grid w-dvw grid-rows-1 overflow-hidden bg-zinc-900"
					style="height: 80dvh; "
				>
					<div
						class="m-4 flex h-full w-screen flex-row gap-8 transition-all"
						style="transform: translate({`${-100 * shopPage}dvw`}); width: 500dvw;"
					>
						{#each { length: 5 } as _, i}
							<div
								class="grid grid-flow-row-dense grid-cols-2 gap-1 overflow-scroll p-1 sm:grid-cols-4"
								style="width: calc(100vw - 2rem); height: calc(100% - 2rem);"
							>
								{#each PageData.shopItems[i] as item}
									<div class="">
										<form
											action="/shop/buyitem/" + {item.itemId}
											method="POST"
											class="flex flex-col items-center rounded-sm border-2 border-zinc-300 bg-zinc-800 p-4 hover:bg-zinc-700"
										>
											<button class="flex flex-col items-center text-zinc-50">
												<div
													class="h-8 w-8 rounded-full border-2 border-zinc-400 p-2 sm:h-16 sm:w-16"
													style="background-color: #bb33ff"
												></div>
												<div class="flex flex-col items-center">
													<span>{item.name}</span>
													<span class="flex flex-row gap-2">
														<Zap />
														{item.price}

														<Hammer />
														{item.buildTime}s
														<Clock />
														{item.baseCooldown ? item.baseCooldown : '-'}
													</span>
													<span>{item.description}</span>
													<!--  TODO! Format this nicelys -->
												</div>
											</button>
										</form>
									</div>
								{/each}
							</div>
						{/each}
					</div>

					<div
						class="absolute bottom-0 left-1/2 flex w-30 flex-row items-center justify-center rounded-t-2xl bg-zinc-900 text-white"
						style="transform: translate(-50%,0);"
					>
						<div class="">
							<button
								class="text-lg"
								aria-label="next"
								onclick={() => {
									shopPage--;
									shopPage %= 5;
									if (shopPage < 0) {
										shopPage = 4;
									}
								}}><ArrowBigLeft /></button
							>
							<button
								class="text-lg"
								aria-label="next"
								onclick={() => {
									shopPage++;
									shopPage %= 5;
								}}><ArrowBigRight /></button
							>
						</div>
					</div>
				</div>
				<button
					class="pointer-events-auto top-0 w-30 cursor-pointer rounded-b-2xl bg-zinc-900 p-2 text-center text-zinc-50 hover:bg-zinc-800 active:bg-zinc-950"
					onclick={() => {
						shopOpen = !shopOpen;
					}}
				>
					SHOP
				</button>
			</div>
		</div>
	{:else}
		<div
			class="justify-cente absolute bottom-0 left-1/2 overflow-hidden transition-all duration-200"
			style="transform: translate(-50%, {joinGameModalOpen
				? '0'
				: `calc(${minimizeJoinGameButtonBox[0].blockSize}px + .5rem)`});"
		>
			<div class="relative flex w-full justify-center">
				<button
					class="bg flex w-25 justify-center rounded-t-sm bg-zinc-950 p-2 text-white hover:bg-zinc-800 active:bg-zinc-600"
					onclick={(joinGameModalOpen = !joinGameModalOpen)}
				>
					{#if joinGameModalOpen}
						<ArrowBigDown />
					{:else}
						<ArrowBigUp />
					{/if}
				</button>
			</div>
			<div
				bind:borderBoxSize={minimizeJoinGameButtonBox}
				class="mb-2 flex flex-col bg-zinc-950 p-2 text-white"
			>
				<p class="text-center">
					You are not currently in the game. If you'd like to join, click the button below. If you'd
					just like to watch, you can minimize this window
				</p>
				<button
					class="my-1 rounded-sm bg-zinc-700 hover:bg-zinc-800 active:bg-zinc-600"
					onclick={joinGame}
				>
					Join game
				</button>
			</div>
		</div>
	{/if}

	{#if selectedNode}
		<div class="tooltip" transition:fade={{ duration: 100 }}>
			<div class="tooltipWrapper">
				<button onclick={() => (selectedNode = null)} class="tooltipCloseButton">X</button>
				<div class="tooltipGrid">
					<h1 class="tooltipHeader">{selectedNode.player.user.username}</h1>
					<form method="POST" use:enhance action="/game/graph/debugconnect">
						<input type="hidden" name="targetUser" value={selectedNode.player.user.username} />
						<button class="bg-gray-600 p-2.5 hover:bg-gray-400" type="submit">Connect</button>
					</form>
				</div>
			</div>
		</div>
	{/if}

	<div id="my_dataviz" class="graphContainer">
		<svg class="graphSVG" bind:this={svgElement}></svg>
	</div>
</div>

<style>
	.graphSVG {
		width: 100%;
		height: 100%;
	}

	.graphContainer {
		height: 100dvh;
		width: 100dvw;
		pointer-events: all;
	}

	.centerStuffPlease {
		position: absolute;
		display: flex;
		align-items: center;
		justify-content: center;
		width: 100vw;
		height: 100vh;
	}

	.errorText {
		font-size: 10rem;
		color: #d32f2f;
	}

	.wrapper {
		width: 100dvw;
		height: 100dvh; /* Fancy new CSS unit I didn't know about */
		position: relative;
		background-color: #212121;
	}

	div.tooltip {
		pointer-events: all;
		position: absolute;
		text-align: center;
		padding: 0.5rem;
		background: #ffffff;
		color: #313639;
		border: 1px solid #313639;
		border-radius: 8px;
		font-size: 1.3rem;
		right: 1vw;
		height: 35vh;
		bottom: 0;
		width: 98vw;
		padding: 3pxu;
	}

	@media only screen and (min-width: 1024px) {
		div.tooltip {
			right: 3px;
			height: 95vh;
			top: 2.5vh;
			width: 20vw;
		}
	}

	:global(div.tooltipWrapper) {
		position: relative;
		width: 100%;
		height: 100%;
	}

	:global(.tooltipCloseButton) {
		position: absolute;
		display: flex;
		top: 5px;
		right: 5px;
		column-count: 4;
		justify-content: center;
		border-color: #212121;
		border-radius: 10px;
		border-width: 4px;
		width: 40px;
		height: 40px;
	}

	:global(.tooltipCloseButton:hover) {
		background-color: #757575;
	}
</style>
