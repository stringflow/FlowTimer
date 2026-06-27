package flowtimer

import "core:fmt"
import "core:slice"
import "core:time"
import "core:c"
import rl "vendor:raylib"

main :: proc() {
    rl.InitWindow(640, 360, "flowtimer")
    defer rl.CloseWindow()

    metronome: Metronome
    create_metronome(&metronome)
    defer delete_metronome(&metronome)

    offsets := []time.Duration{time.Millisecond * 5000, time.Millisecond * 10000}
    max_offset := slice.max(offsets)

    set_beep(&metronome, "beeps/ping1.wav")
    prepare_metronome(&metronome, offsets, time.Millisecond * 500, 5)

    start_tick: time.Tick

    for !rl.WindowShouldClose() {
        rl.BeginDrawing()
        rl.ClearBackground(rl.BLACK)

        if rl.GuiButton({ 20, 60, 150, 30 }, "Start") {
            start_tick = time.tick_now()
            play_metronome(&metronome)
        }

        time_remaining := max_offset
        if start_tick._nsec != 0 {
            time_remaining = max_offset - time.tick_since(start_tick)
            if time_remaining < 0 {
                time_remaining = 0
                start_tick._nsec = 0
            }
        }

        rl.DrawText(rl.TextFormat("%.0f", time.duration_milliseconds(time_remaining)), 20, 90, 32, rl.GRAY)

        rl.EndDrawing()
    }
}
