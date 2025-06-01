<script lang="ts">
	import { onMount } from 'svelte';
	import * as d3 from 'd3';
	import { fade } from 'svelte/transition';
	import Powerbar from '$lib/components/Powerbar.svelte';
	import { enhance } from '$app/forms';
	import ItemBar from '$lib/components/ItemBar.svelte';

	let graph;
	let errored = $state(false);
	let selectedNode = $state(null);

	const { data } = $props();

	let crosshairContainer: d3.Selection<SVGGElement, unknown, HTMLElement, any> | null = null;

	const userPowerLevel = $state({ power: 5 });

	setInterval(() => {
		userPowerLevel.power += 1;
		if (userPowerLevel.power > 10) {
			userPowerLevel.power = 0;
		}
	}, 1000);

	// Change the crosshair display mode when the selected node state changes
	$effect(() => {
		const node = selectedNode; // Variable needs to be accessed for the effect to run
		if (!crosshairContainer) return;

		if (selectedNode) {
			crosshairContainer.style('display', 'block');
		} else {
			crosshairContainer.style('display', 'none');
		}
	});

	onMount(async () => {
		// Container setup
		let screenWidth = window.innerWidth;
		let screenHeight = window.innerHeight;

		// Select container
		const container = d3.select('#my_dataviz');
		container.selectAll('*').remove();

		// Setup the svg and then the render group
		const svg = container.append('svg').attr('width', '100%').attr('height', '100%');
		const mainGroup = svg.append('g');

		// Corsshair setup
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

		// Data loading from the server
		let data;
		try {
			// Load data with promises
			data = await d3.json('/graph');
			if (data == undefined) {
				errored = true;
				return;
			}
		} catch (error) {
			console.error('Error loading data:', error);
			errored = true;
			return;
		}

		// Zoom and drag setup
		const zoom = d3.zoom().on('zoom', (e) => {
			mainGroup.attr('transform', e.transform);
		});

		const drag = d3.drag().on('start', dragstarted).on('drag', dragged).on('end', dragended);

		function dragstarted(event, d) {
			if (!event.active) simulation.alphaTarget(0.3).restart();
			d.fx = event.x;
			d.fy = event.y;
		}

		function dragged(event, d) {
			d.fx = event.x;
			d.fy = event.y;
		}

		function dragended(event, d) {
			if (!event.active) simulation.alphaTarget(0);
			d.fx = null;
			d.fy = null;
		}

		// Create links
		const link = mainGroup
			.selectAll('line')
			.data(data.links)
			.join('line')
			.attr('class', 'node')
			.style('stroke', '#aaa');

		// Create nodes
		const node = mainGroup
			.selectAll('circle')
			.data(data.nodes)
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
			// Drag just exists on the nodes
			.call(drag);

		// Zoom lives on the SVG because you can zoom anywhere
		svg.call(zoom);

		// Force simulation that does the actual layout of the graph
		const simulation = d3
			.forceSimulation(data.nodes)
			.force(
				'link',
				d3
					.forceLink()
					.id((d) => d.id)
					.links(data.links)
			)
			.force('charge', d3.forceManyBody().strength(-400))
			.force('center', d3.forceCenter(screenWidth / 2, screenHeight / 2))
			.on('tick', ticked);

		// General update function for the force simulation
		function ticked() {
			link
				.attr('x1', (d) => d.source.x)
				.attr('y1', (d) => d.source.y)
				.attr('x2', (d) => d.target.x)
				.attr('y2', (d) => d.target.y);

			node.attr('cx', (d) => d.x).attr('cy', (d) => d.y);

			if (selectedNode) {
				// Pull the current position data from our normal dataset to set the position of the crosshair
				const nodeData = data.nodes.find((n) => n.id === selectedNode.id);
				crosshairContainer
					.attr('transform', `translate(${nodeData.x}, ${nodeData.y}) scale(2.25)`)
					.style('display', 'block')
					.raise();
			}
		}
	});
</script>

<div class="wrapper">
	{#if errored}
		<div class="centerStuffPlease">
			<h1 class="errorText">Thigns have exploded :(</h1>
		</div>
	{/if}

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
	</div>

	{#if selectedNode}
		<div class="tooltip" transition:fade={{ duration: 100 }}>
			<div class="tooltipWrapper">
				<button onclick={() => (selectedNode = null)} class="tooltipCloseButton">X</button>
				<div class="tooltipGrid">
					<h1 class="tooltipHeader">{selectedNode.user.username}</h1>
					<form method="POST" use:enhance action="/graph/debugconnect">
						<input type="hidden" name="targetUser" value={selectedNode.user.username} />
						<button class="bg-gray-600 p-2.5 hover:bg-gray-400" type="submit">Connect</button>
					</form>
				</div>
			</div>
		</div>
	{/if}

	<div id="my_dataviz" class="graphContainer"></div>
</div>

<style>
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
		height: 100vh; /* Fancy new CSS unit I didn't know about */
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
