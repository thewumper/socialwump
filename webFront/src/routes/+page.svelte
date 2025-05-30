<script lang="ts">
	import { onMount } from 'svelte';
	import * as d3 from 'd3';
	import { fade } from 'svelte/transition';

	let graph;
	let errored = $state(false);
	let selectedNode = $state(null);
	let currentTransform = d3.zoomIdentity;

	onMount(async () => {
		// Make the callback async
		// Container setup
		let screenWidth = window.innerWidth;
		let screenHeight = window.innerHeight;

		// Select container
		const container = d3.select('#my_dataviz');
		container.selectAll('*').remove();

		// Create SVG
		const svg = container.append('svg').attr('width', '100%').attr('height', '100%');

		const mainGroup = svg.append('g');

		const crosshairContainer = mainGroup
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
			.attr('transform', 'translate(-38, -38)');

		// crosshairContainer
		// 	.append('circle')
		// 	.attr('r', `30`)
		// 	.attr('transform', 'translate(-38, -38)')
		// 	.attr('stroke', 'white')
		// 	.attr('stroke-width', '2')
		// 	.attr('fill', 'none')
		// 	.attr('cx', 0)
		// 	.attr('cy', 0);

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

		const zoom = d3.zoom().on('zoom', (e) => {
			mainGroup.attr('transform', e.transform);
			console.log('zoomeded');
			currentTransform = e.transform;
		});

		const drag = d3.drag().on('start', dragstarted).on('drag', dragged).on('end', dragended);

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
				// crosshairContainer
				// 	.style('display', 'block') // ensure it's visible
				// 	.attr('transform', `translate(${event.x}, ${event.y})`)
				// 	.raise();
			})
			.call(drag);

		svg.call(zoom);

		// Force simulation
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
	});
</script>

<div class="wrapper">
	{#if errored}
		<div class="centerStuffPlease">
			<h1 class="errorText">Thigns have exploded :(</h1>
		</div>
	{/if}

	{#if selectedNode}
		<div class="tooltip" transition:fade={{ duration: 100 }}>
			<div class="tooltipWrapper">
				<button onclick={() => (selectedNode = null)} class="tooltipCloseButton">X</button>
				<div class="tooltipGrid">
					<h1 class="tooltipHeader">{selectedNode.name}</h1>
				</div>
			</div>
		</div>
	{/if}

	<div id="my_dataviz" class="graphContainer"></div>
</div>

<style>
	.graphContainer {
		width: 100vw;
		pointer-events: all;
		height: 100vh;
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
