function Speaker() {
	this.greeting = "Hello";
}

Speaker.prototype.sayHelloTo = function(name) {
	var iAmNotUsed = 0;
	return this.greeting + (name ? " " + name : "") + "!";
};

console.log(new Speaker().speak());