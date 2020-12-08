package main

import (
	"strconv"
	"syscall/js"
)

func add(this js.Value, i []js.Value) interface{} {
	//val1 := i[0].String()
	//val2 := i[1].String()

	val1, _ := strconv.Atoi(i[0].String())
	val2, _ := strconv.Atoi(i[0].String())

	result := val1 + val2
	println(result)
	return result
}

func main() {
	c := make(chan struct{}, 0)

	println("Hello, WebAssembly!")

	js.Global().Set("add", js.FuncOf(add))

	// plotcanv := doc.Call("getElementById", "plotdraw")

	<-c
}
