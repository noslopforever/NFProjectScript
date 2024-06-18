// import { FreeCamera } from "@babylonjs/core/Cameras/freeCamera";
// import { Engine } from "@babylonjs/core/Engines/engine";
// import { HemisphericLight } from "@babylonjs/core/Lights/hemisphericLight";
// import { Vector3 } from "@babylonjs/core/Maths/math.vector";
// import { CreateGround } from "@babylonjs/core/Meshes/Builders/groundBuilder";
// import { CreateSphere } from "@babylonjs/core/Meshes/Builders/sphereBuilder";
// import { Scene } from "@babylonjs/core/scene";

// import { GridMaterial } from "@babylonjs/materials/grid/gridMaterial";

// // Get the canvas element from the DOM.
// const canvas = document.getElementById("renderCanvas");

// // Associate a Babylon Engine to it.
// const engine = new Engine(canvas);

// // Create our first scene.
// var scene = new Scene(engine);

// // This creates and positions a free camera (non-mesh)
// var camera = new FreeCamera("camera1", new Vector3(0, 5, -10), scene);

// // This targets the camera to scene origin
// camera.setTarget(Vector3.Zero());

// // This attaches the camera to the canvas
// camera.attachControl(canvas, true);

// // This creates a light, aiming 0,1,0 - to the sky (non-mesh)
// var light = new HemisphericLight("light1", new Vector3(0, 1, 0), scene);

// // Default intensity is 1. Let's dim the light a small amount
// light.intensity = 0.7;

// // Create a grid material
// var material = new GridMaterial("grid", scene);

// // Our built-in 'sphere' shape.
// var sphere = CreateSphere("sphere1", { segments: 16, diameter: 2 }, scene);

// // Move the sphere upward 1/2 its height
// sphere.position.y = 2;

// // Affect a material
// sphere.material = material;

// // Our built-in 'ground' shape.
// var ground = CreateGround("ground1", { width: 6, height: 6, subdivisions: 2 }, scene);

// // Affect a material
// ground.material = material;

// // Render every frame
// engine.runRenderLoop(() => {
//   scene.render();
// });


var canvas = document.getElementById("renderCanvas");
var createScene = function () {
    var scene = new BABYLON.Scene(engine);
    var camera = new BABYLON.FreeCamera("camera1", new BABYLON.Vector3(0, 5, -10), scene);
		camera.setTarget(BABYLON.Vector3.Zero());
		camera.attachControl(canvas, true);
    var light = new BABYLON.HemisphericLight("light1", new BABYLON.Vector3(0, 1, 0), scene);
		light.intensity = 0.7;
    var ground = BABYLON.Mesh.CreateGround("ground1", 6, 6, 2, scene);
        
	var url;
	var fileName;
		
//-- BoomBox.gltf
	url = "https://raw.githubusercontent.com/BabylonJS/Babylon.js/master/Playground/scenes/BoomBox/";
	fileName = "BoomBox.gltf";

	BABYLON.SceneLoader.ImportMesh("", url, fileName, scene, function (newMeshes) {
		var mesh = newMeshes[0];
			mesh.position.copyFromFloats(0, 1, 0);
			mesh.scaling.copyFromFloats(100,100,100);
			
		camera.target = mesh;
		console.log(mesh)
	});
    return scene;
};

var engine = new BABYLON.Engine(canvas, true, { preserveDrawingBuffer: true, stencil: true });
var scene = createScene();

   engine.runRenderLoop(function () {
	if (scene) {
		scene.render();
	}
});

// Resize
window.addEventListener("resize", function () {
	engine.resize();
});
