<!-- <script>
	import { onMount } from 'svelte';
	import * as d3 from 'd3';
	
	let graph;

	onMount(() => {
	// set the dimensions and margins of the graph
	let margin = {top: 10, right: 30, bottom: 30, left: 40},
	width = 400 - margin.left - margin.right,
	height = 400 - margin.top - margin.bottom;

	// append the svg object to the body of the page
	let svg = d3.select("#my_dataviz")
	.append("svg")
	.attr("width", width + margin.left + margin.right)
	.attr("height", height + margin.top + margin.bottom)
	.append("g")
	.attr("transform",
			"translate(" + margin.left + "," + margin.top + ")");

	d3.json("https://raw.githubusercontent.com/holtzy/D3-graph-gallery/master/DATA/data_network.json", function( data) {

	// Initialize the links
	let link = svg
		.selectAll("line")
		.data(data.links)
		.enter()
		.append("line")
		.style("stroke", "#aaa")

	// Initialize the nodes
	let node = svg
		.selectAll("circle")
		.data(data.nodes)
		.enter()
		.append("circle")
		.attr("r", 20)
		.style("fill", "#69b3a2")

	// Let's list the force we wanna apply on the network
	let simulation = d3.forceSimulation(data.nodes)                 // Force algorithm is applied to data.nodes
		.force("link", d3.forceLink()                               // This force provides links between nodes
				.id(function(d) { return d.id; })                     // This provide  the id of a node
				.links(data.links)                                    // and this the list of links
		)
		.force("charge", d3.forceManyBody().strength(-400))         // This adds repulsion between nodes. Play with the -400 for the repulsion strength
		.force("center", d3.forceCenter(width / 2, height / 2))     // This force attracts nodes to the center of the svg area
		.on("tick", ticked);

	// This function is run at each iteration of the force algorithm, updating the nodes position.
	function ticked() {
		console.log("Hi!")
		link
			.attr("x1", function(d) { return d.source.x; })
			.attr("y1", function(d) { return d.source.y; })
			.attr("x2", function(d) { return d.target.x; })
			.attr("y2", function(d) { return d.target.y; });

		node
			.attr("cx", function (d) { return d.x+6; })
			.attr("cy", function(d) { return d.y-6; });
	}

	

	});	
		
		})

</script> -->

<script>
	import { onMount } from 'svelte';
	import * as d3 from 'd3';
	
	let graph;

	onMount(async () => { // Make the callback async
		// Container setup
		let margin = {top: 10, right: 30, bottom: 30, left: 40},
			width = 400 - margin.left - margin.right,
			height = 400 - margin.top - margin.bottom;

		// Select container
		const container = d3.select("#my_dataviz");

		// Clear previous SVG
		container.selectAll("*").remove();

		// Create SVG
		const svg = container
			.append("svg")
			.attr("width", width + margin.left + margin.right)
			.attr("height", height + margin.top + margin.bottom)
			.append("g")
			.attr("transform", `translate(${margin.left},${margin.top})`);

		try {
			// Load data with promises
			const data = await d3.json("https://raw.githubusercontent.com/holtzy/D3-graph-gallery/master/DATA/data_network.json");

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
				.force("center", d3.forceCenter(width/2, height/2))
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

		} catch (error) {
			console.error("Error loading data:", error);
		}
	});
</script>



<!-- <style>
	.chart :global(div) {
		font: 10px sans-serif;
		background-color: steelblue;
		text-align: right;
		padding: 3px;
		margin: 1px;
		color: white;
	}
</style> -->

<div id="my_dataviz"></div>
