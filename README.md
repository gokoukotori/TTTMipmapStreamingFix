# TTT Mipmap Streaming Fix

`com.gokoukotori.ttt-mipmap-streaming-fix` — version 0.1.0

TexTransTool が生成したテクスチャについて、VRCFury の処理前に Streaming Mipmaps を有効にする回避用 NDMF プラグインです。コンポーネントや設定の追加は不要です。

## 対応環境と導入

- Unity 2022.3（検証: 2022.3.22f1 / Windows / D3D11）
- TexTransTool **1.1.0-beta.9**
- NDMF 1.14.8 以上、2.0.0 未満（検証: 1.14.8）
- 組み合わせの検証対象: VRCFury 1.1429.0

導入すると通常の Play Mode とアバタービルドで自動実行されます。VRCFury が存在しなくても TTT の対象テクスチャには Streaming を設定します。取り外す場合は、このパッケージだけを削除してください。

## 動作

NDMF の Transforming フェーズで TexTransTool の後に実行し、TTT のビルドセッションが保持する生成テクスチャ一覧を読みます。一時生成アセット、MipMap あり、Streaming 無効の Texture2D だけを、その場で有効化します。MLIC に限らず、この時点までに TTT が生成一覧に登録したテクスチャが対象です。

これにより、VRCFury の FixMipmapStreamingService が Streaming 有効化のためにテクスチャを複製する条件から外れます。TTT の置換追跡と最終圧縮設定を維持することが目的です。元画像の Import Settings、テクスチャ内容、フォーマット、MipMap 数は本パッケージでは変更しません。フォーマットは後段の TTT が決定します。

TTT / VRCFury 本体のコード変更、VRCFury の無効化、テクスチャの複製は行いません。NDMF のリアルタイムプレビュー用処理は追加しません。

## 互換性・制限

TTT に生成一覧の公開 API がないため、内部の TexTransBuildSession、RenderersDomain、DownloadedDescriptors を reflection で参照します。そのため TTT の依存を確認済みバージョンに固定しています。構造が一致しない場合は警告して変更を見送ります。テクスチャ名や推測に基づく代替処理は行いません。

NDMF の Transforming → VRCFury → Optimizing という処理順を前提とします。以降の別ツールによる複製や、別バージョンの TTT / VRCFury による変更までは保証しません。
