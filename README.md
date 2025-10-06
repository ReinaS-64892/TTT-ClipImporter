# TTT-ClipImporter

## これはなに？

[TexTransTool PSD Importer](https://ttt.rs64.net/docs/Reference/TexTransToolPSDImporter) のようなレイヤーレベルで [MultiLayerImageCanvas](https://ttt.rs64.net/docs/Reference/MultiLayerImageCanvas) へと PSD からインポートする仕組みを、 Clip Studio Paint の独自形式である `.clip` からインポートを試みる実験的なパッケージです。

## 使い方

この Package を VPM などからインストールした瞬間から、 UnityProject 内の `.clip` が [MLIC](https://ttt.rs64.net/docs/Reference/MultiLayerImageCanvas) とそれに連なるレイヤーとしてインポートされます。(Unity が `.clip` に対するデフォルトインポーターが存在しないため、 TTT-ClipImporter が即使用されるようになります。)(インポーターの選択という概念が存在しないこと以外概ね [TexTransTool PSD Importer](https://ttt.rs64.net/docs/Reference/TexTransToolPSDImporter) と同じような状態になります。)

## インポート可能なレイヤー

現状これらのレイヤーが概ねインポートできているようです。

- RasterLayer (つまり通常の画像レイヤー)
- LayerFolder (レイヤーフォルダー)

## SpecialThanks

- https://github.com/unai-d/unclip
- https://github.com/rasensuihei/cliputils
- https://github.com/nodamushi/clip_layer_rename
