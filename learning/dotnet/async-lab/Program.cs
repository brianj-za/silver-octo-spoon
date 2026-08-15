using System.Diagnostics;

long startTime = Stopwatch.GetTimestamp();

Console.WriteLine(
    $"[Program] start - thread {Environment.CurrentManagedThreadId}, t = {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds} ms"
);

var task = FetchUserAsync(42);
Console.WriteLine(
    $"[Program] after fetch - thread {Environment.CurrentManagedThreadId}, t = {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds} ms"
);

var user = await task;
Console.WriteLine(
    $"[Program] after await - thread {Environment.CurrentManagedThreadId}, t = {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds} ms"
);

return;

async Task<string> FetchUserAsync(int id)
{
    Console.WriteLine(
        $"[FetchUserAsync] entered - thread {Environment.CurrentManagedThreadId}, t = {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds} ms"
    );
    await Task.Delay(500);
    Console.WriteLine(
        $"[FetchUserAsync] resumed, thread {Environment.CurrentManagedThreadId}, t = {Stopwatch.GetElapsedTime(startTime).TotalMilliseconds} ms"
    );

    return $"User ID: {id}";
}
