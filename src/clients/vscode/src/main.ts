'use strict';

import vscode = require('vscode');
import { SessionManager } from './session';

var sessionManager: SessionManager = undefined;

export function activate(context: vscode.ExtensionContext): void {
    context.subscriptions.push(
        vscode.commands.registerCommand(
            "ShaderTools.OpenLogFolder",
            () => {
                context.logger.logDirectory.then(x => {
                    vscode.commands.executeCommand(
                        "vscode.openFolder",
                        vscode.Uri.file(x),
                        true);
                });
            }));

    context.subscriptions.push(sessionManager = new SessionManager(context.logger));
    sessionManager.start();
}