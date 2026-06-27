package flowtimer

import "core:mem"
import "core:slice"
import "core:time"
import ma "vendor:miniaudio"

AUDIO_FORMAT :: ma.format.f32
AUDIO_SAMPLE_RATE :: 48000
AUDIO_CHANNELS :: 2
AUDIO_PERIOD_IN_FRAMES :: 16
AUDIO_BYTES_PER_FRAME := ma.get_bytes_per_frame(AUDIO_FORMAT, AUDIO_CHANNELS)

BUFFER_COUNT :: 2

Audio :: struct {
    pcm: []u8,
    frame_count: u32,
}

Metronome :: struct {
    device: ma.device,

    queued: ^Audio,
    frame_position: u32,

    buffers: [BUFFER_COUNT]Audio,
    buffer_index: int,
    beep: Audio,
}

metronome_data_proc :: proc "c" (device: ^ma.device, sink, _: rawptr, requested_frames: u32) {
    mem.zero(sink, int(audio_frames_to_bytes(requested_frames)))

    metronome := transmute(^Metronome)device.pUserData
    if metronome.queued == nil do return

    remaining := metronome.queued.frame_count - metronome.frame_position
    frames_to_copy := min(requested_frames, remaining)

    if frames_to_copy > 0 {
        src_offset := audio_frames_to_bytes(metronome.frame_position)
        mem.copy(sink, &metronome.queued.pcm[src_offset], int(audio_frames_to_bytes(frames_to_copy)))
        metronome.frame_position += frames_to_copy
    }
}

audio_frames_to_bytes :: proc "contextless" (frame_count: u32) -> u32 {
    return frame_count * AUDIO_BYTES_PER_FRAME
}

duration_to_audio_frames :: proc "contextless" (duration: time.Duration) -> u32 {
    return u32(time.duration_seconds(duration) * f64(AUDIO_SAMPLE_RATE))
}

delete_audio :: proc(audio: Audio) {
	delete(audio.pcm)
}

resize_audio :: proc(audio: ^Audio, frame_count: u32) {
    delete_audio(audio^)
    audio.pcm = make([]u8, audio_frames_to_bytes(frame_count))
    audio.frame_count = frame_count
}

create_metronome :: proc(metronome: ^Metronome) -> bool {
    config := ma.device_config_init(.playback)
    config.periodSizeInFrames = AUDIO_PERIOD_IN_FRAMES
    config.playback.format = AUDIO_FORMAT
    config.playback.channels = AUDIO_CHANNELS
    config.sampleRate = AUDIO_SAMPLE_RATE
    config.dataCallback = metronome_data_proc
    config.pUserData = metronome

    if ma.device_init(nil, &config, &metronome.device) != .SUCCESS do return false
    if ma.device_start(&metronome.device) != .SUCCESS do return false

    return true
}

delete_metronome :: proc(metronome: ^Metronome) -> bool {
	for buffer in metronome.buffers do delete_audio(buffer)
    delete_audio(metronome.beep)

    if ma.device_stop(&metronome.device) != .SUCCESS do return false
    ma.device_uninit(&metronome.device)

    return true
}

set_beep :: proc(metronome: ^Metronome, filepath: cstring) -> bool {
    config := ma.decoder_config_init(AUDIO_FORMAT, AUDIO_CHANNELS, AUDIO_SAMPLE_RATE)

    decoder: ma.decoder
    if ma.decoder_init_file(filepath, &config, &decoder) != .SUCCESS do return false
    defer ma.decoder_uninit(&decoder)

    frame_count: u64
    if ma.decoder_get_length_in_pcm_frames(&decoder, &frame_count) != .SUCCESS do return false
    resize_audio(&metronome.beep, u32(frame_count))

    frames_read: u64
    if ma.decoder_read_pcm_frames(&decoder, raw_data(metronome.beep.pcm), u64(metronome.beep.frame_count), &frames_read) != .SUCCESS do return false
    if frames_read != frame_count do return false

    return true
}

prepare_metronome :: proc(metronome: ^Metronome, offsets: []time.Duration, interval: time.Duration, beeps: int) {
	metronome.buffer_index = (metronome.buffer_index + 1) % BUFFER_COUNT
	target_buffer := &metronome.buffers[metronome.buffer_index]

    max_offset := slice.max(offsets)

    frame_count := duration_to_audio_frames(max_offset) + metronome.beep.frame_count
    resize_audio(target_buffer, frame_count)

    for offset in offsets {
        for i in 0..<beeps {
            offset_duration := offset - time.Duration(i) * interval
            offset_frames := duration_to_audio_frames(offset_duration)
            offset_byte := audio_frames_to_bytes(offset_frames)
            copy(target_buffer.pcm[offset_byte:], metronome.beep.pcm)
        }
    }
}

play_audio :: proc(metronome: ^Metronome, audio: ^Audio) {
    metronome.queued = audio
    metronome.frame_position = 0
}

play_metronome :: proc(metronome: ^Metronome) {
	play_audio(metronome, &metronome.buffers[metronome.buffer_index])
}
