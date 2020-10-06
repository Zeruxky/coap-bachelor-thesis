public CompletableFuture<Integer> calculateAsync() throws InterruptedException {
    CompletableFuture<Integer> completableFuture = CompletableFuture.supplyAsync(() -> {
        Thread.sleep(1000);
        return 1;
    });
    return completableFuture;
}

public static void main(string[] args) {
    CompletableFuture cf = calculateAsync();
    while (!cf.isDone) {
        System.out.println("CompleteableFuture is not finished yet...");
    }
    long result = cf.get();
}