<script>
	import { onMount } from 'svelte';
	import * as d3 from 'd3';
	
	let graph;
	let errored = $state(false);

	onMount(async () => { // Make the callback async
		// Container setup
		let screenWidth = window.innerWidth;
		let screenHeight = window.innerHeight;


		// Select container
		const container = d3.select("#my_dataviz");

		// Clear previous SVG
		container.selectAll("*").remove();

		// Create SVG
		const svg = container
			.append("svg")
			.attr("width", "100%")
			.attr("height", "100%")
			.append("g")

		let data;
		try {
			// Load data with promises
			data  = await d3.json("/graph");
		} catch (error) {
			console.error("Error loading data:", error);
			return
		}

		// Create links
		const link = svg
			.selectAll("line")
			.data(data.links)
			.join("line")
			.style("stroke", "#aaa");

		// Create nodes
		const node = svg
			.selectAll("circle")
			.data(data.nodes)
			.join("circle")
			.attr("r", 20)
			.style("fill", "#69b3a2");

		// Force simulation
		const simulation = d3.forceSimulation(data.nodes)
			.force("link", d3.forceLink()
				.id(d => d.id)
				.links(data.links)
			)
			.force("charge", d3.forceManyBody().strength(-400))
			.force("center", d3.forceCenter(screenWidth/2, screenHeight/2))
			.on("tick", ticked);

		function ticked() {
			link
				.attr("x1", d => d.source.x)
				.attr("y1", d => d.source.y)
				.attr("x2", d => d.target.x)
				.attr("y2", d => d.target.y);

			node
				.attr("cx", d => d.x)
				.attr("cy", d => d.y);
		}
	});
</script>



<style>
	.graphContainer {
		width: 100vw;
		height: 100vh;
	}
</style>

{#if errored}
	<div>
		<h1>Thigns have exploded :(</h1>
	</div>
{/if}

<div id="my_dataviz" class="graphContainer"></div>
