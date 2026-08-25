import Foundation
import Testing

// Every test below is a deliberate flake generator, so a red run here is the
// point rather than a regression. Each uses a different pattern: an
// unconditional failure, random draws at fixed rates, a clock-driven window, a
// thrown error, and passing tests whose duration moves.
//
// Keep a blank line between any comment and the @Test or @Suite under it. Swift
// Testing reads a contiguous preceding comment as a test comment and reprints
// it beneath every failure, which buries the actual message.
//
// JUnit identity is the function and struct name — `classname` is
// "MyCLITests.<struct>" and `name` is "<function>()". The display strings below
// never reach the XML, so renaming one is free and renaming a function is not.

@Suite("best tests evar") struct MyCoolTests {
    @Test("rad test name") func helloworld() {
        let greeting = "hello, world!"
        #expect(greeting == "hello, worldzx!")
    }
}

// Fixed pass rates, so detection sees a stable proportion rather than a trend.
// Spaced widely enough that each test's history is visibly its own: 90% reads
// as flaky, 50% as a coin toss, 10% as broken-with-exceptions.

@Suite("swift pass rate ladder") struct PassRateLadder {
    @Test("passes 90 percent of runs") func ninetyPercent() {
        expectPass(rate: 90)
    }

    @Test("passes 75 percent of runs") func seventyFivePercent() {
        expectPass(rate: 75)
    }

    @Test("passes 50 percent of runs") func fiftyPercent() {
        expectPass(rate: 50)
    }

    @Test("passes 25 percent of runs") func twentyFivePercent() {
        expectPass(rate: 25)
    }

    @Test("passes 10 percent of runs") func tenPercent() {
        expectPass(rate: 10)
    }

    private func expectPass(rate: Int) {
        let draw = Int.random(in: 0 ..< 100)
        #expect(draw < rate, "drew \(draw), which needed to be under \(rate)")
    }
}

// Clock-driven rather than random, so failures cluster instead of scattering:
// every run inside the same half-minute agrees. The Go suite keys on second
// parity and flips every second, so this flips every 30 to stay distinct.

@Suite("swift wall clock") struct WallClockTests {
    @Test("fails in the first half of every minute") func firstHalfOfMinute() {
        let second = Calendar.current.component(.second, from: Date())
        #expect(second >= 30, "second \(second) is in the first half of the minute")
    }

    @Test("fails on odd-numbered minutes") func oddMinute() {
        let minute = Calendar.current.component(.minute, from: Date())
        #expect(minute % 2 == 0, "minute \(minute) is odd")
    }
}

// A thrown error rather than a failed expectation. Both land in JUnit as
// <failure> — Swift Testing's xunit output never emits <error>, so this differs
// from its neighbours in where the failure comes from, not in how it reports.

@Suite("swift thrown errors") struct ThrownErrorTests {
    private struct BrewingFailure: Error, CustomStringConvertible {
        let temperature: Int
        var description: String { "kettle reached \(temperature)C, needed 100C" }
    }

    @Test("throws roughly a third of the time") func throwsIntermittently() throws {
        let temperature = Int.random(in: 90 ... 101)
        guard temperature >= 100 else {
            throw BrewingFailure(temperature: temperature)
        }
    }
}

// Always pass; only the duration moves. That isolates the duration signal from
// the failure signal, so a monitor reacting here is reacting to runtime alone.
// Every other test in this package finishes in microseconds.

@Suite("swift durations") struct DurationTests {
    @Test("takes somewhere between 50ms and 1.5s") func variableDuration() async throws {
        try await Task.sleep(for: .milliseconds(Int.random(in: 50 ... 1500)))
    }

    @Test("takes twice as long on odd-numbered hours") func hourlyDuration() async throws {
        let hour = Calendar.current.component(.hour, from: Date())
        try await Task.sleep(for: .milliseconds(hour % 2 == 0 ? 200 : 400))
    }
}
