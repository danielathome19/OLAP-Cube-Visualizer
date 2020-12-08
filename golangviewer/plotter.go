package main

// package main

import (
	"fmt"
)

func headermenu(headers []string) {
	println("Header options:")
	for i := 0; i < len(headers); i++ {
		fmt.Printf("\t%d. %s\n", i, headers[i])
	}
}

//func Equal[T comparable]
//Won't work because it doesn't support dynamic typing
func main() {
	fmt.Println("Hello world!")
	headers := []string{"0", "1", "2", "3", "4"}

	headermenu(headers)
}
